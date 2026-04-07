using Microsoft.Extensions.Options;
using MxCaptcha.Options;
using MxCaptcha.Services;
using Xunit;

namespace MxRecaptcha.Tests;

public class MxCaptchaServiceTests
{
    [Fact]
    public async Task GenerateAsync_StoresCodeAndReturnsDataUri()
    {
        var options = Options.Create(new MxCaptchaOptions { Expiration = TimeSpan.FromMinutes(3) });
        var store = new FakeCaptchaStore();
        var codeGenerator = new FakeCaptchaCodeGenerator("a1B2c");
        var imageGenerator = new FakeCaptchaImageGenerator([1, 2, 3, 4]);
        var sut = new MxCaptchaService(options, store, codeGenerator, imageGenerator);

        var result = await sut.GenerateAsync();

        Assert.NotNull(result.Id);
        Assert.Equal(32, result.Id.Length);
        Assert.StartsWith("data:image/png;base64,", result.Base64Image);
        Assert.Equal("data:image/png;base64,AQIDBA==", result.Base64Image);
        Assert.True(store.Values.ContainsKey(result.Id));
        Assert.Equal("a1B2c", store.Values[result.Id].Code);
        Assert.Equal("a1B2c", imageGenerator.LastCode);
        Assert.Same(options.Value, imageGenerator.LastOptions);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenInputIsInvalid()
    {
        var options = Options.Create(new MxCaptchaOptions());
        var store = new FakeCaptchaStore();
        var sut = new MxCaptchaService(options, store, new FakeCaptchaCodeGenerator("abc"), new FakeCaptchaImageGenerator([1]));

        Assert.False(await sut.ValidateAsync("", "abc"));
        Assert.False(await sut.ValidateAsync("id", ""));
        Assert.False(await sut.ValidateAsync(" ", "abc"));
        Assert.False(await sut.ValidateAsync("id", " "));
        Assert.Equal(0, store.RemoveCallCount);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalse_WhenCaptchaDoesNotExist()
    {
        var options = Options.Create(new MxCaptchaOptions());
        var store = new FakeCaptchaStore();
        var sut = new MxCaptchaService(options, store, new FakeCaptchaCodeGenerator("abc"), new FakeCaptchaImageGenerator([1]));

        var valid = await sut.ValidateAsync("missing-id", "abc");

        Assert.False(valid);
        Assert.Equal(0, store.RemoveCallCount);
    }

    [Fact]
    public async Task ValidateAsync_IsCaseInsensitive_WhenConfigured()
    {
        var options = Options.Create(new MxCaptchaOptions { CaseSensitive = false });
        var store = new FakeCaptchaStore();
        await store.SetAsync("id-1", "AbC12", DateTime.UtcNow.AddMinutes(1));
        var sut = new MxCaptchaService(options, store, new FakeCaptchaCodeGenerator("abc"), new FakeCaptchaImageGenerator([1]));

        var valid = await sut.ValidateAsync("id-1", "aBc12");

        Assert.True(valid);
        Assert.Equal(1, store.RemoveCallCount);
        Assert.False(store.Values.ContainsKey("id-1"));
    }

    [Fact]
    public async Task ValidateAsync_IsCaseSensitive_WhenConfigured()
    {
        var options = Options.Create(new MxCaptchaOptions { CaseSensitive = true });
        var store = new FakeCaptchaStore();
        await store.SetAsync("id-2", "AbC12", DateTime.UtcNow.AddMinutes(1));
        var sut = new MxCaptchaService(options, store, new FakeCaptchaCodeGenerator("abc"), new FakeCaptchaImageGenerator([1]));

        var valid = await sut.ValidateAsync("id-2", "aBc12");

        Assert.False(valid);
        Assert.Equal(0, store.RemoveCallCount);
        Assert.True(store.Values.ContainsKey("id-2"));
    }

    [Fact]
    public async Task RefreshAsync_ReplacesStoredCaptcha_ForExistingId()
    {
        var options = Options.Create(new MxCaptchaOptions { Expiration = TimeSpan.FromMinutes(5) });
        var store = new FakeCaptchaStore();
        var codeGenerator = new FakeCaptchaCodeGenerator("new-code");
        var sut = new MxCaptchaService(options, store, codeGenerator, new FakeCaptchaImageGenerator([1]));

        await sut.RefreshAsync("known-id");

        Assert.Equal(1, codeGenerator.GenerateCallCount);
        Assert.True(store.Values.ContainsKey("known-id"));
        Assert.Equal("new-code", store.Values["known-id"].Code);
    }

    [Fact]
    public async Task RefreshAsync_DoesNothing_ForEmptyId()
    {
        var options = Options.Create(new MxCaptchaOptions());
        var store = new FakeCaptchaStore();
        var codeGenerator = new FakeCaptchaCodeGenerator("new-code");
        var sut = new MxCaptchaService(options, store, codeGenerator, new FakeCaptchaImageGenerator([1]));

        await sut.RefreshAsync("");

        Assert.Equal(0, codeGenerator.GenerateCallCount);
        Assert.Empty(store.Values);
    }
}
