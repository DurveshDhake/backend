var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapHub<ChatHub>("/chathub");

app.Run();
