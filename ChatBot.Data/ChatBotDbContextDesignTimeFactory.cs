using Microsoft.EntityFrameworkCore.Design;


namespace ChatBot.Data;

public class ChatBotDbContextDesignTimeFactory
    : IDesignTimeDbContextFactory<ChatBotDbContext>
{
    private const string ASP_NET_CORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
    private const string DEFAULT_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

    public ChatBotDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        return Create(basePath, Environment.GetEnvironmentVariable(ASP_NET_CORE_ENVIRONMENT) ?? DEFAULT_ENVIRONMENT);
    }

    private ChatBotDbContext Create(string basePath, string environmentName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .Build();

        var readWriteConnectionString = configuration.GetConnectionString(ChatBotDbContext.CONNECTION_STRING_NAME);

        return Create(readWriteConnectionString);
    }

    private ChatBotDbContext Create(string? readWriteConnectionString)
    {
        if (string.IsNullOrWhiteSpace(readWriteConnectionString))
        {
            throw new ArgumentException($"Connection string is null or empty.", nameof(readWriteConnectionString));
        }

        var assemblyName = typeof(ChatBotDbContext).Assembly.GetName().Name;

        var optionsBuilder = new DbContextOptionsBuilder<ChatBotDbContext>();

        // If you need more information during migration build, uncomment the above line
        //optionsBuilder.EnableSensitiveDataLogging();

        optionsBuilder.UseSqlServer(readWriteConnectionString, o =>
        {
            o.MigrationsAssembly(assemblyName);
            o.CommandTimeout(1800);
        });

        return new ChatBotDbContext(optionsBuilder.Options);
    }
}
