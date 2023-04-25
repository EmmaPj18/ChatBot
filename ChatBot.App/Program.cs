using ChatBot.App.Areas.Identity;
using ChatBot.App.Hubs;
using ChatBot.App.Services;
using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Core.Interface;
using ChatBot.Core.Services;
using ChatBot.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddChatBotDbContext(builder.Configuration);

builder.Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChatBotDbContext>();

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddSingleton<IBotCommandService, BotCommandService>();
builder.Services.AddSingleton<IBotCommandRequestService, BotCommandRequestService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddHostedService<BotResponseBackgroundService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<ChatRoomHub>(HubConstants.CHAT_ROOM_HUB);

app.Run();
