namespace ChatBot.Core.Models;

public sealed record ChatMessageViewModel(DateTimeOffset SendAt, string Message, int UserId, string UserName);
