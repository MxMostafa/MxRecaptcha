
using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace MxCaptcha.Services;

public class SkiaSharpCaptchaImageGenerator : ICaptchaImageGenerator
{
    private readonly MxCaptchaOptions _options;
    private readonly Random _random = new();

    public SkiaSharpCaptchaImageGenerator(IOptions<MxCaptchaOptions> options)
    {
        _options = options.Value;
    }

    public byte[] Generate(string code, MxCaptchaOptions opt)
    {
        using var bitmap = new SKBitmap(opt.ImageWidth, opt.ImageHeight);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.White);

        DrawNoiseDots(canvas, opt);
        DrawNoiseLines(canvas, opt);
        DrawCharacters(canvas, code, opt);

        if (opt.UseWaveDistortion)
        {
            ApplyWaveDistortion(bitmap, opt);
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }

    // -------------------------------
    // DRAW CHARACTERS
    // -------------------------------
    private void DrawCharacters(SKCanvas canvas, string code, MxCaptchaOptions opt)
    {
        using var paint = new SKPaint
        {
            TextSize = opt.FontSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(opt.FontFamily ?? "Arial", SKFontStyle.Bold)
        };

        float xPos = 25;

        foreach (char c in code)
        {
            paint.Color = opt.UseRandomTextColor
                ? GetRandomColor()
                : SKColors.Black;

            float rotation = 0;

            if (opt.UseRandomRotation)
            {
                rotation = (float)(_random.NextDouble() * opt.MaxRotationDegrees * 2 - opt.MaxRotationDegrees);
            }

            canvas.Save();

            canvas.Translate(xPos, opt.ImageHeight / 2);
            canvas.RotateDegrees(rotation);

            canvas.DrawText(c.ToString(), 0, 0, paint);

            canvas.Restore();

            xPos += opt.CharacterSpacing;
        }
    }

    private SKColor GetRandomColor()
    {
        return new SKColor(
            (byte)_random.Next(20, 180),
            (byte)_random.Next(20, 180),
            (byte)_random.Next(20, 180));
    }

    // --------------------------------
    // NOISE DOTS
    // --------------------------------
    private void DrawNoiseDots(SKCanvas canvas, MxCaptchaOptions opt)
    {
        using var paint = new SKPaint { IsAntialias = true };

        int count = opt.NoiseLevel * 150;

        for (int i = 0; i < count; i++)
        {
            paint.Color = GetRandomColor();

            float x = _random.Next(opt.ImageWidth);
            float y = _random.Next(opt.ImageHeight);

            canvas.DrawPoint(x, y, paint);
        }
    }

    // --------------------------------
    // NOISE LINES
    // --------------------------------
    private void DrawNoiseLines(SKCanvas canvas, MxCaptchaOptions opt)
    {
        using var paint = new SKPaint
        {
            StrokeWidth = 1.5f,
            IsStroke = true,
            IsAntialias = true
        };

        for (int i = 0; i < opt.NoiseLevel; i++)
        {
            paint.Color = GetRandomColor();

            float x1 = _random.Next(opt.ImageWidth);
            float y1 = _random.Next(opt.ImageHeight);

            float x2 = _random.Next(opt.ImageWidth);
            float y2 = _random.Next(opt.ImageHeight);

            canvas.DrawLine(x1, y1, x2, y2, paint);
        }
    }

    // --------------------------------
    // SIMPLE WAVE DISTORTION
    // --------------------------------
    private void ApplyWaveDistortion(SKBitmap bitmap, MxCaptchaOptions opt)
    {
        int width = bitmap.Width;
        int height = bitmap.Height;

        var temp = new SKBitmap(width, height);

        float amp = opt.WaveAmplitude;
        float freq = opt.WaveFrequency;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int newX = (int)(x + Math.Sin(y / freq) * amp);
                int newY = (int)(y + Math.Cos(x / freq) * amp);

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    temp.SetPixel(x, y, bitmap.GetPixel(newX, newY));
                }
            }
        }

        using var canvas = new SKCanvas(bitmap);
        canvas.DrawBitmap(temp, 0, 0);
    }
}

