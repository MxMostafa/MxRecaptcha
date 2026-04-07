using MxCaptcha.Options;
using MxRecaptcha.Services;
using Xunit;

namespace MxRecaptcha.Tests;

public class SkiaCaptchaImageGeneratorTests
{
    [Fact]
    public void Generate_ReturnsPngBytes()
    {
        var options = new MxCaptchaOptions
        {
            ImageWidth = 180,
            ImageHeight = 60,
            NoiseLevel = 1
        };

        var sut = new SkiaCaptchaImageGenerator();

        var result = sut.Generate("A1b2", options);

        Assert.NotEmpty(result);
        Assert.True(result.Length > 8);
        Assert.Equal(0x89, result[0]);
        Assert.Equal((byte)'P', result[1]);
        Assert.Equal((byte)'N', result[2]);
        Assert.Equal((byte)'G', result[3]);
    }
}
