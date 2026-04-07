using MxCaptcha.Options;

namespace MxCaptcha.Abstractions;

public interface ICaptchaCodeGenerator
{
    string GenerateCode(MxCaptchaOptions options);
}
