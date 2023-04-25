namespace ChatBot.Core.Interface;

public interface IRabbitMqService : IDisposable
{
    Task Consume<T>(string queue, Func<T, CancellationToken, Task> execute, CancellationToken ct = default);
    Task Produce<T>(string queue, T message, CancellationToken ct = default);
}
