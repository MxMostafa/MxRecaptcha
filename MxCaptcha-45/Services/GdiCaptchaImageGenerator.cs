using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;

namespace MxCaptcha.Services
{
    public class GdiCaptchaImageGenerator : ICaptchaImageGenerator
    {
        private static readonly string[] FallbackFonts = { "Arial", "Tahoma", "Times New Roman" };

        private readonly Random _random = new Random();

        public byte[] Generate(string code, MxCaptchaOptions options)
        {
            code = code ?? string.Empty;

            using (var bitmap = new Bitmap(options.ImageWidth, options.ImageHeight))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.White);

                DrawNoiseDots(bitmap, options);
                DrawNoiseLines(graphics, options);
                DrawCharacters(graphics, code, options);

                if (options.UseWaveDistortion)
                {
                    ApplyWaveDistortion(bitmap, options);
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private void DrawCharacters(Graphics graphics, string code, MxCaptchaOptions options)
        {
            var family = ResolveFontFamily(options.FontFamily);
            using (var font = new Font(family, options.FontSize, FontStyle.Bold))
            {
                var xPos = Math.Max(8f, options.CharacterSpacing / 2f);
                var yPos = Math.Max(2f, (options.ImageHeight - options.FontSize) / 2f);

                foreach (var c in code)
                {
                    var color = options.UseRandomTextColor ? GetRandomColor() : Color.Black;
                    var rotation = options.UseRandomRotation
                        ? (float)(_random.NextDouble() * options.MaxRotationDegrees * 2 - options.MaxRotationDegrees)
                        : 0f;

                    graphics.TranslateTransform(xPos, yPos);
                    if (Math.Abs(rotation) > 0.01f)
                    {
                        graphics.RotateTransform(rotation);
                    }

                    using (var brush = new SolidBrush(color))
                    {
                        graphics.DrawString(c.ToString(), font, brush, 0, 0);
                    }

                    graphics.ResetTransform();
                    xPos += options.CharacterSpacing;
                }
            }
        }

        private static FontFamily ResolveFontFamily(string requested)
        {
            FontFamily family;
            if (TryCreateFontFamily(requested, out family))
            {
                return family;
            }

            for (var i = 0; i < FallbackFonts.Length; i++)
            {
                if (TryCreateFontFamily(FallbackFonts[i], out family))
                {
                    return family;
                }
            }

            return FontFamily.GenericSansSerif;
        }

        private static bool TryCreateFontFamily(string name, out FontFamily family)
        {
            family = null;

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            try
            {
                family = new FontFamily(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Color GetRandomColor()
        {
            return Color.FromArgb(_random.Next(20, 180), _random.Next(20, 180), _random.Next(20, 180));
        }

        private void DrawNoiseDots(Bitmap bitmap, MxCaptchaOptions options)
        {
            var count = options.NoiseLevel * 150;
            for (var i = 0; i < count; i++)
            {
                var x = _random.Next(bitmap.Width);
                var y = _random.Next(bitmap.Height);
                bitmap.SetPixel(x, y, Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
            }
        }

        private void DrawNoiseLines(Graphics graphics, MxCaptchaOptions options)
        {
            for (var i = 0; i < options.NoiseLevel; i++)
            {
                using (var pen = new Pen(GetRandomColor(), 1.6f))
                {
                    var p1 = new Point(_random.Next(options.ImageWidth), _random.Next(options.ImageHeight));
                    var p2 = new Point(_random.Next(options.ImageWidth), _random.Next(options.ImageHeight));
                    graphics.DrawLine(pen, p1, p2);
                }
            }
        }

        private static void ApplyWaveDistortion(Bitmap bitmap, MxCaptchaOptions options)
        {
            if (options.WaveFrequency <= 0f || options.WaveAmplitude == 0f)
            {
                return;
            }

            using (var source = (Bitmap)bitmap.Clone())
            {
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
                            bitmap.SetPixel(x, y, Color.White);
                        }
                    }
                }
            }
        }
    }
}
