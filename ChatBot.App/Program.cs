using ChatBot.App.Hubs;
using ChatBot.Core.Constants;
using ChatBot.Core.Entities;
using ChatBot.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString() 
    ?? throw new InvalidOperationException($"Connection string '{ChatBotDbContext.CONNECTION_STRING_NAME}' not found.");

builder.Services.AddDbContext<ChatBotDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChatBotDbContext>();

// Add services to the container.
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
