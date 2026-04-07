using MxCaptcha.Options;
using MxCaptcha.Services;
using Xunit;

namespace MxRecaptcha.Tests;

public class RandomCaptchaCodeGeneratorTests
{
    [Fact]
    public void GenerateCode_ReturnsConfiguredLength()
    {
        var options = new MxCaptchaOptions
        {
            CodeLength = 12,
            UseNumbers = true,
            UseLetters = true,
            CaseSensitive = true
        };
        var sut = new RandomCaptchaCodeGenerator();

        var code = sut.GenerateCode(options);

        Assert.Equal(12, code.Length);
    }

    [Fact]
    public void GenerateCode_UsesOnlyDigits_WhenNumbersEnabledOnly()
    {
        var options = new MxCaptchaOptions
        {
            CodeLength = 30,
            UseNumbers = true,
            UseLetters = false
        };
        var sut = new RandomCaptchaCodeGenerator();

        var code = sut.GenerateCode(options);

        Assert.All(code, ch => Assert.InRange(ch, '0', '9'));
    }

    [Fact]
    public void GenerateCode_UsesLowercaseLetters_WhenCaseInsensitive()
    {
        var options = new MxCaptchaOptions
        {
            CodeLength = 30,
            UseNumbers = false,
            UseLetters = true,
            CaseSensitive = false
        };
        var sut = new RandomCaptchaCodeGenerator();

        var code = sut.GenerateCode(options);

        Assert.All(code, ch => Assert.InRange(ch, 'a', 'z'));
    }

    [Fact]
    public void GenerateCode_UsesLettersAndNumbers_WhenBothEnabled()
    {
        var options = new MxCaptchaOptions
        {
            CodeLength = 100,
            UseNumbers = true,
            UseLetters = true,
            CaseSensitive = true
        };
        var sut = new RandomCaptchaCodeGenerator();

        var code = sut.GenerateCode(options);

        Assert.All(code, ch =>
            Assert.True(char.IsDigit(ch) || (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')));
    }

    [Fact]
    public void GenerateCode_Throws_WhenNoCharacterSetsEnabled()
    {
        var options = new MxCaptchaOptions
        {
            UseNumbers = false,
            UseLetters = false
        };
        var sut = new RandomCaptchaCodeGenerator();

        var ex = Assert.Throws<InvalidOperationException>(() => sut.GenerateCode(options));

        Assert.Equal("No characters configured for captcha generation.", ex.Message);
    }
}
