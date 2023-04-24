using Microsoft.AspNetCore.SignalR;

namespace ChatBot.App.Hubs;

public class ChatRoomHub : Hub
{
    public async Task Broadcast(
        string? userName,
        int userId,
        string? displayName,
        string message
        )
    {
        await Clients.All.SendAsync("Broadcast",
            userName,
            userId,
            displayName,
            message);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected {Context.ConnectionId} {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }
}
