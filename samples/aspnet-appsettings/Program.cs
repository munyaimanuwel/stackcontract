using StackContract.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseStackContractKeyCheck(new[]
{
    "ConnectionStrings:Default"
});
var app = builder.Build();
app.MapGet("/", () => "ok");
app.Run();
