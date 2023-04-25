using ChatBot.App.Hubs;
using ChatBot.Core.Constants;
using ChatBot.Core.Interface;
using ChatBot.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatBot.App.Services;

public class BotResponseBackgroundService : BackgroundService
{
    private readonly ILogger<BotResponseBackgroundService> _logger;
    private readonly IHubContext<ChatRoomHub> _chatRoomHubContext;
    private readonly IRabbitMqService _rabbitMqService;

    public BotResponseBackgroundService(
        IRabbitMqService rabbitMqService, 
        IHubContext<ChatRoomHub> chatRoomHubContext, 
        ILoggerFactory loggerFactory)
    {
        _chatRoomHubContext = chatRoomHubContext;
        _logger = loggerFactory.CreateLogger<BotResponseBackgroundService>();
        _rabbitMqService = rabbitMqService;
    }

    private async Task WaitForBotResponse(CancellationToken ct)
    {
        await _rabbitMqService.Consume<ChatMessageViewModel>(QueueNames.CHAT_QUEUE, async (botMsg, ct) =>
        {
            await _chatRoomHubContext.Clients.All.SendAsync(HubConstants.CHAT_ROOM_METHOD, botMsg, cancellationToken: ct);
        }, ct);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _logger.LogInformation("Waiting for Bot Response");
        await WaitForBotResponse(ct);
    }
}
