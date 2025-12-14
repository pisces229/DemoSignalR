using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using RazorPage;
using RazorPage.Hubs;
using RazorPage.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<BackgroundHostedService>();
builder.Services.AddSingleton<MessageRepository>();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

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
        // get the value from "Context.User.Identity.Name".
        NameClaimType = ClaimTypes.NameIdentifier,
        // make [Authorize] determine the role.
        RoleClaimType = ClaimTypes.Role,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("CC5B1B93EDA34B788DD2743CA39BAF89")),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
            {
                Console.WriteLine("OnMessageReceived");
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
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

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("HubRestricted", policy =>
//    {
//        policy.Requirements.Add(new HubRestrictedRequirement());
//    });
//});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//app.Use(async (context, next) =>
//{
//    if (context.Request != null)
//    {
//        if (context.Request.Path.Value != null)
//        {
//            if (context.Request.Path.Value.StartsWith("/hubs"))
//            {
//                var bearerToken = context.Request.Query["access_token"].ToString();
//                if (context.Request.Headers != null && !string.IsNullOrEmpty(bearerToken))
//                {
//                    Console.WriteLine("access_token to Authorization Bearer");
//                    context.Request.Headers.Append("Authorization", "Bearer " + bearerToken);
//                }
//            }
//        }
//    }
//    await next();
//});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<AuthorizeHub>("/hubs/AuthorizeHub");
app.MapHub<BackgroundHub>("/hubs/BackgroundHub");
app.MapHub<BroadcastHub>("/hubs/BroadcastHub");
app.MapHub<CustomerHub>("/hubs/CustomerHub");
app.MapHub<GroupHub>("/hubs/GroupHub");
app.MapHub<HomeHub>("/hubs/HomeHub");

app.Run();
