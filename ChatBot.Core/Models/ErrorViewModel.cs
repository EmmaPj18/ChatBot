namespace ChatBot.Core.Models;

public sealed record ErrorViewModel(string? RequestId, string? Message)
{
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
