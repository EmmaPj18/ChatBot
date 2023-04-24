using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatBot.App.Pages;

public partial class ChatRoom : ComponentBase
{
    private bool _isChatting;
    private string _username = "TEST";
    private int _userid = 2;

    private string? _newMessage;
    private string? _errorMessage;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private UserManager<User> UserManager { get; set; } = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = default!;

    private List<ChatMessageViewModel> Messages { get; set; } = new();

    private HubConnection? HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationState;
        var userClaimPrincipal = authState.User;
        var user = await UserManager.GetUserAsync(userClaimPrincipal);

        if (!userClaimPrincipal.Identity?.IsAuthenticated ?? false
            && user == null)
        {
            _errorMessage = $"ERROR: You need to log in first to access the bot";
            _isChatting = false;
            return;
        }

        _username = user!.UserName!;
        _userid = user.Id;

        try
        {
            // Start chatting and force refresh UI.
            _isChatting = true;
            await Task.Delay(1);

            // remove old messages if any
            Messages.Clear();

            // Create the chat client
            string baseUrl = NavigationManager.BaseUri;

            var hubUrl = baseUrl.TrimEnd('/') + HubConstants.CHAT_ROOM_HUB;

            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            HubConnection.On<string, int, string>("Broadcast", BroadcastMessage);

            await HubConnection.StartAsync();

            await Send($"[Notice] {_username} joined chat room.", HubConstants.CHAT_BOT_NAME, HubConstants.CHAT_BOT_ID);
        }
        catch (Exception e)
        {
            _errorMessage = $"ERROR: Failed to start chat client: {e.Message}";
            _isChatting = false;
        }
    }

    private void BroadcastMessage(string userName, int userId, string message)
    {
        Messages.Add(new ChatMessageViewModel(DateTimeOffset.UtcNow, message, userId, userName));

        // Inform blazor the UI needs updating
        InvokeAsync(StateHasChanged);
    }

    private async Task Disconnect()
    {
        if (!_isChatting)
        {
            return;
        }

        await Send($"[Notice] {_username} left chat room.", HubConstants.CHAT_BOT_NAME, HubConstants.CHAT_BOT_ID);

        await HubConnection!.StopAsync();
        await HubConnection!.DisposeAsync();

        HubConnection = null;
        _isChatting = false;
    }

    private async Task Send(string message, string userName, int userId)
    {
        if (!_isChatting)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        await HubConnection!.SendAsync("Broadcast", userName, userId, message);

        _newMessage = string.Empty;
    }

}
