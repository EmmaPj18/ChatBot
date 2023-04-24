using ChatBot.Core.Constants;

namespace ChatBot.Core.Models;

public sealed record ChatMessageViewModel(DateTimeOffset SendAt, string Message, int UserId, string? UserName, string? DisplayName)
{
    private bool IsNotice => Message.StartsWith("[Notice]");

    public string ApplyCSS => UserId == HubConstants.CHAT_BOT_ID
        ? (IsNotice ? "notice" : "received")
        : "sent";
}
