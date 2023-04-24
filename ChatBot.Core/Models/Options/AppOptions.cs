namespace ChatBot.Core.Models.Options;

public sealed class AppOptions
{
    public RabbitMqOptions RabbitMqOptions { get; set; } = new();
}
