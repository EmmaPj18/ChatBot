// See https://aka.ms/new-console-template for more information
using ChatBot.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChatBot.Data;
using ChatBot.Core.Interface;
using ChatBot.Core.Services;
using ChatBot.Bot.Options;
using Microsoft.Extensions.Configuration;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();
        services.AddChatBotDbContext(hostContext.Configuration);
        services.AddSingleton<IRabbitMqService, RabbitMqService>();

        services.Configure<StockApiOptions>(hostContext.Configuration.GetRequiredSection(StockApiOptions.STOCK_API_OPTIONS));

        services.AddHostedService<BotRequestBackgroundService>();
    })
    .Build();

await host.StartAsync();
await host.WaitForShutdownAsync();