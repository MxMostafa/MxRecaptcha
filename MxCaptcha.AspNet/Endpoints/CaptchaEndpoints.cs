using MxCaptcha.Abstractions;
using MxCaptcha.AspNet.Models;

namespace MxCaptcha.AspNet.Endpoints;

public static class CaptchaEndpoints
{
    public static void MapCaptchaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/captcha", async (IMxCaptchaService captcha) =>
        {
            var result = await captcha.GenerateAsync();

            return Results.Ok(result);
        });

        app.MapPost("/captcha/verify", async (
            IMxCaptchaService captcha,
            CaptchaVerifyRequest request) =>
        {
            var isValid = await captcha.ValidateAsync(request.Id, request.Code);

            return Results.Ok(new { success = isValid });
        });
    }
}
