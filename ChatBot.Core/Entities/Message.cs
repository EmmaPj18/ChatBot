namespace ChatBot.Core.Entities;

public sealed class ChatMessage
{
    public int Id { get; set; }
    public required string Message { get; set; }
    public DateTimeOffset SendAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = default!;
}
