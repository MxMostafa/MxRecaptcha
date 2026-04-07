

namespace MxCaptcha.Models;

public class MxCaptchaResult
{
    public string Id { get; set; } = default!;

    public string Base64Image { get; set; } = default!;

    public DateTime ExpireAt { get; set; }
}
