using Microsoft.Extensions.Options;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingFont = System.Drawing.Font;
using DrawingFontFamily = System.Drawing.FontFamily;
using DrawingFontStyle = System.Drawing.FontStyle;
using DrawingGraphics = System.Drawing.Graphics;
using DrawingPen = System.Drawing.Pen;
using DrawingPoint = System.Drawing.Point;
using DrawingSolidBrush = System.Drawing.SolidBrush;
using SmoothingMode = System.Drawing.Drawing2D.SmoothingMode;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace MxRecaptcha.Tests;

internal sealed class GdiCaptchaImageGenerator : ICaptchaImageGenerator
{
    private static readonly string[] FallbackFonts = ["DejaVu Sans", "Liberation Sans", "Arial", "Tahoma"];

    private readonly string _fontFamily;
    private readonly float _fontSize;
    private readonly Random _random = new();

    public GdiCaptchaImageGenerator(IOptions<MxCaptchaOptions> options)
    {
        var configured = options.Value;
        _fontFamily = ResolveFontFamilyName(configured.FontFamily);
        _fontSize = configured.FontSize;
    }

    public byte[] Generate(string code, MxCaptchaOptions options)
    {
        code ??= string.Empty;

        using var bitmap = new DrawingBitmap(options.ImageWidth, options.ImageHeight);
        using var graphics = DrawingGraphics.FromImage(bitmap);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(DrawingColor.White);

        DrawNoiseDots(bitmap, options);
        DrawNoiseLines(graphics, options);
        DrawCharacters(graphics, code, options);

        if (options.UseWaveDistortion)
        {
            ApplyWaveDistortion(bitmap, options);
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    private static string ResolveFontFamilyName(string requestedFont)
    {
        if (TryCreateFontFamily(requestedFont, out var family))
        {
            family.Dispose();
            return requestedFont;
        }

        foreach (var fallback in FallbackFonts)
        {
            if (TryCreateFontFamily(fallback, out family))
            {
                family.Dispose();
                return fallback;
            }
        }

        return DrawingFontFamily.GenericSansSerif.Name;
    }

    private static bool TryCreateFontFamily(string? name, out DrawingFontFamily family)
    {
        family = default!;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        try
        {
            family = new DrawingFontFamily(name);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void DrawCharacters(DrawingGraphics graphics, string code, MxCaptchaOptions options)
    {
        using var font = new DrawingFont(_fontFamily, _fontSize, DrawingFontStyle.Bold);
        float xPos = Math.Max(8f, options.CharacterSpacing / 2f);
        float yPos = Math.Max(2f, (options.ImageHeight - _fontSize) / 2f);

        foreach (var c in code)
        {
            var color = options.UseRandomTextColor ? GetRandomColor() : DrawingColor.Black;
            var rotation = options.UseRandomRotation
                ? (float)(_random.NextDouble() * options.MaxRotationDegrees * 2 - options.MaxRotationDegrees)
                : 0f;

            graphics.TranslateTransform(xPos, yPos);
            if (Math.Abs(rotation) > 0.01f)
            {
                graphics.RotateTransform(rotation);
            }

            using var brush = new DrawingSolidBrush(color);
            graphics.DrawString(c.ToString(), font, brush, 0, 0);
            graphics.ResetTransform();

            xPos += options.CharacterSpacing;
        }
    }

    private DrawingColor GetRandomColor()
    {
        return DrawingColor.FromArgb(
            _random.Next(20, 180),
            _random.Next(20, 180),
            _random.Next(20, 180));
    }

    private void DrawNoiseDots(DrawingBitmap bitmap, MxCaptchaOptions options)
    {
        var count = options.NoiseLevel * 150;
        for (var i = 0; i < count; i++)
        {
            var x = _random.Next(bitmap.Width);
            var y = _random.Next(bitmap.Height);
            bitmap.SetPixel(x, y, DrawingColor.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
        }
    }

    private void DrawNoiseLines(DrawingGraphics graphics, MxCaptchaOptions options)
    {
        for (var i = 0; i < options.NoiseLevel; i++)
        {
            using var pen = new DrawingPen(GetRandomColor(), 1.6f);
            var p1 = new DrawingPoint(_random.Next(options.ImageWidth), _random.Next(options.ImageHeight));
            var p2 = new DrawingPoint(_random.Next(options.ImageWidth), _random.Next(options.ImageHeight));
            graphics.DrawLine(pen, p1, p2);
        }
    }

    private static void ApplyWaveDistortion(DrawingBitmap bitmap, MxCaptchaOptions options)
    {
        if (options.WaveFrequency <= 0f || options.WaveAmplitude == 0f)
        {
            return;
        }

        using var source = (DrawingBitmap)bitmap.Clone();

        var amplitude = options.WaveAmplitude;
        var frequency = options.WaveFrequency;

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var sourceX = (int)(x + Math.Sin(y / frequency) * amplitude);
                var sourceY = (int)(y + Math.Cos(x / frequency) * amplitude);

                if (sourceX >= 0 && sourceX < bitmap.Width && sourceY >= 0 && sourceY < bitmap.Height)
                {
                    bitmap.SetPixel(x, y, source.GetPixel(sourceX, sourceY));
                }
                else
                {
                    bitmap.SetPixel(x, y, DrawingColor.White);
                }
            }
        }
    }
}
