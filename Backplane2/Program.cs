using Backplane;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("https://localhost:7198")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
})
// same namespace Backplane
.AddStackExchangeRedis("localhost:6379", options =>
 {
     options.Configuration.ChannelPrefix = RedisChannel.Literal("indexHub");
 });

var app = builder.Build();

app.UseCors("CorsPolicy");

app.MapHub<IndexHub>("/hubs/IndexHub");

app.MapGet("/", () => "Backplane2");

app.Run();
