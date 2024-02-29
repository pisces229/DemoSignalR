using RazorPage.Hubs;
using RazorPage.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MessageRepository>();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.MapHub<IndexHub>("/indexHub");
app.MapHub<GroupHub>("/GroupHub");
app.MapHub<CustomerHub>("/CustomerHub");

app.Run();
