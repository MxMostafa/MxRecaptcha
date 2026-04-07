using MxCaptcha.Options;

namespace MxCaptcha.Abstractions;

public interface ICaptchaImageGenerator
{
    byte[] Generate(string code, MxCaptchaOptions options);
}
