using Microsoft.Extensions.DependencyInjection;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using MxCaptcha.Services;
using MxRecaptcha.Services;
using System;
using System.Collections.Generic;
using System.Text;

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
        services.AddSingleton<ICaptchaImageGenerator, ImageSharpCaptchaImageGenerator>();
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

