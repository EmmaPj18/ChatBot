using ChatBot.Core.Constants;
using ChatBot.Core.Interface;
using ChatBot.Core.Models;
using Microsoft.Extensions.Logging;

namespace ChatBot.Core.Services;

public class BotCommandRequestService : IBotCommandRequestService
{
    private readonly ILogger<BotCommandRequestService> _logger;
    private readonly IRabbitMqService _rabbitMqService;

    public BotCommandRequestService(ILogger<BotCommandRequestService> logger, IRabbitMqService rabbitMqService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
    }

    public async Task ExecuteCommand(CommandInformation command, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending to queue the request for {stock}", command);
        await _rabbitMqService.Produce(QueueNames.BOT_QUEUE, command, ct);
    }
}
