using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Core.Models;
using ChatBot.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.App.Pages;

public partial class ChatRoom : ComponentBase, IAsyncDisposable
{
    private bool _isChatting;
    private string? _username;
    private int _userid;
    private string? _displayName;

    private string? _newMessage;
    private string? _errorMessage;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private UserManager<User> UserManager { get; set; } = default!;
    [Inject]
    private ChatBotDbContext Context { get; set; } = default!;

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

        _username = user!.UserName;
        _displayName = user!.DisplayName;
        _userid = user.Id;

        try
        {
            // Start chatting and force refresh UI.
            _isChatting = true;
            await Task.Delay(1);

            // remove old messages if any
            Messages.Clear();

            var messages = await Context.ChatMessages
                .AsNoTracking()
                .Include(x => x.User)
                .OrderByDescending(x => x.SendAt)
                .Take(50)
                .ToListAsync();

            if (messages.Any())
            {
                Messages.AddRange(messages.Select(x => new ChatMessageViewModel(x.SendAt, x.Message, x.UserId, x.User.UserName, x.User.DisplayName)));
            }

            // Create the chat client
            string baseUrl = NavigationManager.BaseUri;

            var hubUrl = baseUrl.TrimEnd('/') + HubConstants.CHAT_ROOM_HUB;

            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            HubConnection.On<string?, int, string?, string>(HubConstants.CHAT_ROOM_METHOD, BroadcastMessage);

            await HubConnection.StartAsync();

            await SendBotMessage($"[Notice] {_displayName}({_username}) joined chat room.");
        }
        catch (Exception e)
        {
            _errorMessage = $"ERROR: Failed to start chat client: {e.Message}";
            _isChatting = false;
        }
    }

    private void BroadcastMessage(string? userName, int userId, string? displayName, string message)
    {
        var chatMessage = new ChatMessageViewModel(DateTimeOffset.UtcNow, message, userId, userName, displayName);

        Messages.Add(chatMessage);

        // Inform blazor the UI needs updating
        InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isChatting)
        {
            return;
        }

        await SendBotMessage($"[Notice] {_displayName}({_username}) left chat room.");

        await HubConnection!.StopAsync();
        await HubConnection!.DisposeAsync();

        HubConnection = null;
        _isChatting = false;
    }

    private async Task SendBotMessage(string message)
    {
        var chatMessage = new ChatMessageViewModel(
            DateTimeOffset.UtcNow,
            message,
            HubConstants.CHAT_BOT_ID,
            HubConstants.CHAT_BOT_MAIL,
            HubConstants.CHAT_BOT_NAME);

        await SendInternal(chatMessage);
    }

    private async Task SendMessage(string? userName, int userId, string? displayName, string message)
    {
        var chatMessage = new ChatMessageViewModel(
            DateTimeOffset.UtcNow,
            message,
            userId,
            userName,
            displayName);

        await SendInternal(chatMessage);
    }

    private async Task SendInternal(ChatMessageViewModel chatMessage)
    {
        if (!_isChatting)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(chatMessage.Message))
        {
            return;
        }

        await HubConnection!.SendAsync(HubConstants.CHAT_ROOM_METHOD, chatMessage);

        _newMessage = string.Empty;
    }
}
