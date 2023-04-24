
using ChatBot.Core.Entities;

namespace ChatBot.Data;

public class ChatBotDbContext : DbContext
{
    public const string CONNECTION_STRING_NAME = "DefaultConnection";

    public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatBotDbContext).Assembly);
    }
}