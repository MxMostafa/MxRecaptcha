using Microsoft.Extensions.DependencyInjection;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using MxCaptcha.Services;
using MxRecaptcha.Services;

namespace MxCaptcha.Extensions;

public static class MxCaptchaServiceCollectionExtensions
{
    public static IServiceCollection AddMxCaptcha(
        this IServiceCollection services,
        Action<MxCaptchaOptions>? configure = null)
    {
        if (configure != null)
            services.Configure(configure);

        services.AddMemoryCache();
        services.AddSingleton<IMxCaptchaService, MxCaptchaService>();
        services.AddSingleton<ICaptchaCodeGenerator, RandomCaptchaCodeGenerator>();
        services.AddSingleton<ICaptchaImageGenerator, SkiaSharpCaptchaImageGenerator>();
        services.AddSingleton<ICaptchaStore, MemoryCaptchaStore>();

        return services;
    }


    public static IServiceCollection UseRedisStore(this IServiceCollection services)
    {
        services.AddSingleton<ICaptchaStore, DistributedCaptchaStore>();

        return services;
    }


    public static IServiceCollection AddMxCaptchaSkiaSharp(
     this IServiceCollection services)
    {
        services.AddSingleton<ICaptchaImageGenerator, SkiaCaptchaImageGenerator>();
        return services;
    }
}

