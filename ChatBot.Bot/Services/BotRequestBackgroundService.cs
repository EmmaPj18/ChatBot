using ChatBot.Bot.Models;
using ChatBot.Bot.Options;
using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Core.Interface;
using ChatBot.Core.Models;
using ChatBot.Data;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ChatBot.Bot.Services;

public class BotRequestBackgroundService : IHostedService
{
    private readonly ILogger<BotRequestBackgroundService> _logger;
    private readonly StockApiOptions _stockApiOptions;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ChatBotDbContext _chatBotDbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public BotRequestBackgroundService(
        ILogger<BotRequestBackgroundService> logger,
        IOptions<StockApiOptions> stockApiOptions,
        IRabbitMqService rabbitMqService,
        IHttpClientFactory httpClientFactory,
        ChatBotDbContext chatBotDbContext)
    {
        _logger = logger;
        _stockApiOptions = stockApiOptions.Value;
        _rabbitMqService = rabbitMqService;
        _httpClientFactory = httpClientFactory;
        _chatBotDbContext = chatBotDbContext;

        if (string.IsNullOrWhiteSpace(_stockApiOptions.Url))
            throw new ArgumentNullException(nameof(_stockApiOptions.Url));

    }

    private async Task ResolveStockCode(CommandInformation command, CancellationToken ct = default)
    {
        var message = command.Command switch
        {
            BotCommandConstants.STOCK_COMMAND => await GetStockMessage(command.Parameter, ct),
            _ => $"CommandInfo {command} is not a Stock Command"
        };

        await ProduceBotMessage(message, ct);
    }

    public async Task ProduceBotMessage(string message, CancellationToken ct = default)
    {
        var chatMessage = new ChatMessageViewModel(DateTimeOffset.Now, message, HubConstants.CHAT_BOT_ID, HubConstants.CHAT_BOT_MAIL, HubConstants.CHAT_BOT_NAME);

        chatMessage = await SaveMessage(chatMessage);

        _logger.LogInformation("Sending Chat Message: {msg}", chatMessage);
        await _rabbitMqService.Produce(QueueNames.CHAT_QUEUE, chatMessage, ct);
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

    private async Task<string> GetStockMessage(string parameter, CancellationToken ct)
    {
        try
        {
            var url = $"{_stockApiOptions.Url}?s={parameter}&f=sd2t2ohlcv&h&e=csv";

            using var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetStreamAsync(url, ct);

            using var reader = new StreamReader(response);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!await csvReader.ReadAsync())
                throw new Exception("Could not read the stock csv");

            var stockData = csvReader.GetRecord<StockData>();

            if (stockData == null || stockData.Close == "N/D")
                throw new Exception("Data cannot be found for the stock code");

            return $"{stockData.Symbol} quote is ${stockData.Close} per share.";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error trying to read stock for code {parameter}. {message}", parameter, e.Message);

            return $"Error trying to read stock for code {parameter}. {e.Message}";
        }
    }

    public async Task StartAsync(CancellationToken ct)
    {
        _logger.LogInformation($"Setting Consume Queue Method on function {nameof(StartAsync)}");
        await _rabbitMqService.Consume<CommandInformation>(QueueNames.BOT_QUEUE, ResolveStockCode, ct);
    }

    public Task StopAsync(CancellationToken ct)
    {
        _rabbitMqService.Dispose();
        return Task.CompletedTask;
    }
}
