using Backplane.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfiguration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build());

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "Issuer",
        ValidAudience = "Audience",
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]!)),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/Hub"))
            {
                Console.WriteLine("OnMessageReceived");
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                } 
                else
                {
                    context.Fail("Invalid access token.");
                }
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("OnTokenValidated");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("OnAuthenticationFailed");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            Console.WriteLine("OnForbidden");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    //options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.DisableImplicitFromServicesParameters = true;
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("Redis")))
{
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis")!, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("Hub");
        });
}

builder.Services.AddSingleton<IHubService, HubService>();

builder.Services.AddSingleton<TokenService>();

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ClientSendHub>("/Hub");

app.MapGet("/", (IHttpContextAccessor _httpContextAccessor)
    => $"{_httpContextAccessor.HttpContext?.Request?.Host} Backplane");

app.MapGet("/Api/AdminToken", (TokenService tokenService) 
    => new { token = tokenService.CreateToken("Admin", TimeSpan.FromDays(1)) });

app.MapGet("/Api/UserToken", (TokenService tokenService) 
    => new { token = tokenService.CreateToken("User", TimeSpan.FromDays(1)) });

app.MapGet("/Api/ExpiredToken", (TokenService tokenService)
    => new { token = tokenService.CreateToken("Admin", TimeSpan.FromSeconds(1)) });

app.MapGet("/Api/Send", (IHttpContextAccessor _httpContextAccessor, IHubService hubService, IConfiguration configuration)
    => hubService.Send($"{configuration["Name"]} Send"));

app.Run();
