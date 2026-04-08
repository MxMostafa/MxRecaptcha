using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using MxCaptcha.Services;

namespace MxCaptcha.AspNet45
{
    internal static class CaptchaRuntime
    {
        private static readonly IMxCaptchaService ServiceInstance = MxCaptchaFactory.Create(new MxCaptchaOptions
        {
            FontFamily = "Arial",
            ImageWidth = 200,
            ImageHeight = 60
        });

        public static IMxCaptchaService Service
        {
            get { return ServiceInstance; }
        }
    }
}
