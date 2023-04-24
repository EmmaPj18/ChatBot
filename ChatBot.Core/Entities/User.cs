using Microsoft.AspNetCore.Identity;

namespace ChatBot.Core.Entities;

public sealed class User : IdentityUser<int>
{
    public required string DisplayName { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = new HashSet<ChatMessage>();
}