using Microsoft.Extensions.DependencyInjection;

namespace ChatBot.Data;

public static class ServiceHost
{
    public static IServiceCollection AddChatBotDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ChatBotDbContext.CONNECTION_STRING_NAME)
                ?? throw new InvalidOperationException($"Connection string '{ChatBotDbContext.CONNECTION_STRING_NAME}' not found.");

        services.AddDbContext<ChatBotDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
}
