using ChatBot.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Data.Configuration;

public sealed class ChatMessagesConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);
        builder.Property(x => x.Message)
            .IsRequired();
        builder.Property(x => x.SendAt)
            .HasDefaultValueSql("getdate()");
        builder.Property(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
