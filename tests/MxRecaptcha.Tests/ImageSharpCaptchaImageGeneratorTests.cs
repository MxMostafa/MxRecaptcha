using Microsoft.Extensions.Options;
using MxCaptcha.Options;
using MxCaptcha.Services;
using Xunit;

namespace MxRecaptcha.Tests;

public class ImageSharpCaptchaImageGeneratorTests
{
    [Fact]
    public void Generate_ReturnsPngBytes()
    {
        var options = Options.Create(new MxCaptchaOptions
        {
            ImageWidth = 180,
            ImageHeight = 60,
            NoiseLevel = 1,
            FontSize = 24,
            UseRandomRotation = false,
            UseWaveDistortion = false,
            UseRandomTextColor = false
        });

        var sut = new ImageSharpCaptchaImageGenerator(options);

        var result = sut.Generate("A1b2", options.Value);

        Assert.NotEmpty(result);
        Assert.True(result.Length > 8);
        Assert.Equal(0x89, result[0]);
        Assert.Equal((byte)'P', result[1]);
        Assert.Equal((byte)'N', result[2]);
        Assert.Equal((byte)'G', result[3]);
    }
}
