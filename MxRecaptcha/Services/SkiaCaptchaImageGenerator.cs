using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using SkiaSharp;

namespace MxRecaptcha.Services;

public class SkiaCaptchaImageGenerator : ICaptchaImageGenerator
{
    public byte[] Generate(string code, MxCaptchaOptions options)
    {
        var rand = new Random();

        using var surface = SKSurface.Create(new SKImageInfo(options.ImageWidth, options.ImageHeight));
        var canvas = surface.Canvas;

        canvas.Clear(SKColors.White);

        var paint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 32,
            IsAntialias = true
        };

        // draw noise dots
        for (int i = 0; i < options.NoiseLevel * 150; i++)
        {
            canvas.DrawPoint(
                rand.Next(options.ImageWidth),
                rand.Next(options.ImageHeight),
                new SKPaint { Color = SKColors.Gray });
        }

        // draw random lines
        for (int i = 0; i < options.NoiseLevel; i++)
        {
            var linePaint = new SKPaint
            {
                Color = SKColors.DarkGray,
                StrokeWidth = 2
            };

            canvas.DrawLine(
                rand.Next(options.ImageWidth),
                rand.Next(options.ImageHeight),
                rand.Next(options.ImageWidth),
                rand.Next(options.ImageHeight),
                linePaint);
        }

        canvas.DrawText(code, 20, 40, paint);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }
}
