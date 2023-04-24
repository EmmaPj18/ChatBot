using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Data;

public class ChatBotDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public const string CONNECTION_STRING_NAME = "DefaultConnection";

    public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
    {

    }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(e => e.DisplayName)
           .IsRequired();

        modelBuilder.Entity<User>().HasIndex(e => e.DisplayName)
            .IsUnique();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatBotDbContext).Assembly);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        SeedBotUser(modelBuilder.Entity<User>());
    }

    private void SeedBotUser(EntityTypeBuilder<User> builder)
    {
        var botUser = new User
        {
            Id = HubConstants.CHAT_BOT_ID,
            Email = HubConstants.CHAT_BOT_MAIL,
            EmailConfirmed = true,
            UserName = HubConstants.CHAT_BOT_MAIL,
            DisplayName = HubConstants.CHAT_BOT_NAME
        };

        var ph = new PasswordHasher<User>();
        botUser.PasswordHash = ph.HashPassword(botUser, "lMWHr0xuVfC^");

        builder.HasData(botUser);
    }
}