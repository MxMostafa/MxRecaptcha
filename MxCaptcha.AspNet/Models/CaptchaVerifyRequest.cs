namespace MxCaptcha.AspNet.Models;

public class CaptchaVerifyRequest
{
    public string Id { get; set; } = default!;
    public string Code { get; set; } = default!;
}
