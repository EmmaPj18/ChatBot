using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Core.Interface;
using ChatBot.Core.Models;
using ChatBot.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.App.Hubs;

public class ChatRoomHub : Hub
{
    private readonly ILogger<ChatRoomHub> _logger;
    private readonly IBotCommandService _botCommandService;
    private readonly ChatBotDbContext _chatBotDbContext;
    private readonly IBotCommandRequestService _botCommandRequestService;


    public ChatRoomHub(
        ILogger<ChatRoomHub> logger,
        IBotCommandRequestService botCommandRequestService,
        IBotCommandService botCommandService,
        ChatBotDbContext chatBotDbContext)
    {
        _logger = logger;
        _botCommandRequestService = botCommandRequestService;
        _botCommandService = botCommandService;
        _chatBotDbContext = chatBotDbContext;
    }

    public async Task Broadcast(
        ChatMessageViewModel chatMessage
        )
    {
        if (_botCommandService.IsCommand(chatMessage.Message))
        {
            await ExecuteCommandInternal(chatMessage);

            return;
        }

        chatMessage = await SaveMessage(chatMessage);

        await BroadcastInternal(chatMessage);
    }

    private async Task ExecuteCommandInternal(ChatMessageViewModel chatMessage)
    {
        try
        {
            await BroadcastInternal(chatMessage);

            var commandMessage = _botCommandService.GetCommandInformation(chatMessage.Message);

            if (!string.IsNullOrWhiteSpace(commandMessage.Error))
            {
                chatMessage = await SaveMessage(ConstructBotChatMessage(commandMessage.Error));

                await BroadcastInternal(chatMessage);

                return;
            }

            await _botCommandRequestService.ExecuteCommand(commandMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Information Invalid Command : {CommandText}", chatMessage.Message);
            return;
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogWarning($"{Context.ConnectionId} connected");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogWarning($"Disconnected {Context.ConnectionId} {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<ChatMessageViewModel> SaveMessage(ChatMessageViewModel chatMessage)
    {
        var user = await _chatBotDbContext.Users
            .Where(x => x.Id == chatMessage.UserId)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return ConstructBotChatMessage("User not found");
        }

        var message = new ChatMessage
        {
            Message = chatMessage.Message,
            UserId = chatMessage.UserId,
            User = user,
        };

        _chatBotDbContext.ChatMessages.Add(message);
        await _chatBotDbContext.SaveChangesAsync();

        chatMessage = chatMessage with { SendAt = message.SendAt };

        return chatMessage;

    }

    private static ChatMessageViewModel ConstructBotChatMessage(string message)
    {
        var chatMessage = new ChatMessageViewModel(
            DateTimeOffset.UtcNow,
            message,
            HubConstants.CHAT_BOT_ID,
            HubConstants.CHAT_BOT_MAIL,
            HubConstants.CHAT_BOT_NAME);

        return chatMessage;
    }

    private async Task BroadcastInternal(
        ChatMessageViewModel chatMessage,
        CancellationToken ct = default
    )
    {
        _logger.LogInformation("Broadcasting message : {chatMessage}", chatMessage);
        await Clients.All.SendAsync(HubConstants.CHAT_ROOM_METHOD, chatMessage, ct);
    }
}
