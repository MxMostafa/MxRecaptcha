using MxCaptcha.AspNet.Endpoints;
using MxCaptcha.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMxCaptcha(op =>
{
    op.FontFamily = "Arial";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCaptchaEndpoints();

app.Run();
