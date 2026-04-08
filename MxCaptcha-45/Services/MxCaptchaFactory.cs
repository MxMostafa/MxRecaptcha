using MxCaptcha.Abstractions;
using MxCaptcha.Options;

namespace MxCaptcha.Services
{
    public static class MxCaptchaFactory
    {
        public static IMxCaptchaService Create(MxCaptchaOptions options = null)
        {
            var captchaOptions = options ?? new MxCaptchaOptions();
            return new MxCaptchaService(
                captchaOptions,
                new MemoryCaptchaStore(),
                new RandomCaptchaCodeGenerator(),
                new GdiCaptchaImageGenerator());
        }
    }
}
