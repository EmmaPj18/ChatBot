using ChatBot.Core.Constants;
using ChatBot.Core.Models;
using Microsoft.AspNetCore.Components;
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

    private List<ChatMessageViewModel> Messages { get; set; } = new();

    private HubConnection HubConnection { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Validate auth and set _username and _userid

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

        await HubConnection.StopAsync();
        await HubConnection.DisposeAsync();

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

        await HubConnection.SendAsync("Broadcast", userName, userId, message);

        _newMessage = string.Empty;
    }

}
