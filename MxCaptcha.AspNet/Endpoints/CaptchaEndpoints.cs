using MxCaptcha.Abstractions;
using MxCaptcha.AspNet.Models;

namespace MxCaptcha.AspNet.Endpoints;

public static class CaptchaEndpoints
{
    public static void MapCaptchaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/captcha")
            .WithTags("Captcha");

        group.MapGet("/", async (IMxCaptchaService captcha) =>
        {
            var result = await captcha.GenerateAsync();

            return Results.Ok(result);
        })
        .WithName("GenerateCaptcha")
        .WithSummary("Generates a captcha image and identifier.")
        .Produces(StatusCodes.Status200OK);

        group.MapPost("/verify", async (
            IMxCaptchaService captcha,
            CaptchaVerifyRequest request) =>
        {
            var isValid = await captcha.ValidateAsync(request.Id, request.Code);

            return Results.Ok(new { success = isValid });
        })
        .WithName("VerifyCaptcha")
        .WithSummary("Validates a captcha answer.")
        .Produces(StatusCodes.Status200OK);
    }
}
