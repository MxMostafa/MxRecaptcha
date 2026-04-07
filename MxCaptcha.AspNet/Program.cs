using MxCaptcha.AspNet.Endpoints;
using MxCaptcha.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddMxCaptcha(op =>
{
    op.FontFamily = "Arial";
});
var app = builder.Build();
app.MapCaptchaEndpoints();

app.Run();
