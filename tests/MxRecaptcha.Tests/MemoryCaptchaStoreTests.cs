using Microsoft.Extensions.DependencyInjection;
using MxCaptcha.Abstractions;
using MxCaptcha.Extensions;
using Xunit;

namespace MxRecaptcha.Tests;

public class MemoryCaptchaStoreTests
{
    [Fact]
    public async Task GetAsync_RemovesValue_AfterRead()
    {
        await using var provider = CreateProvider();
        var store = provider.GetRequiredService<ICaptchaStore>();
        var expiration = DateTime.UtcNow.AddMinutes(1);

        await store.SetAsync("captcha-1", "abc", expiration);

        var firstRead = await store.GetAsync("captcha-1");
        var secondRead = await store.GetAsync("captcha-1");

        Assert.Equal("abc", firstRead);
        Assert.Null(secondRead);
    }

    [Fact]
    public async Task RemoveAsync_DeletesValue()
    {
        await using var provider = CreateProvider();
        var store = provider.GetRequiredService<ICaptchaStore>();

        await store.SetAsync("captcha-2", "value", DateTime.UtcNow.AddMinutes(1));
        await store.RemoveAsync("captcha-2");
        var value = await store.GetAsync("captcha-2");

        Assert.Null(value);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddMxCaptcha();
        return services.BuildServiceProvider();
    }
}
