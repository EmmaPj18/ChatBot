using ChatBot.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatBot.Data.Configuration;

public sealed class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.DisplayName)
            .IsRequired();

        builder.HasIndex(e => e.DisplayName)
            .IsUnique();
    }
}
