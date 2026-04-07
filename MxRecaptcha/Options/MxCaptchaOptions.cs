
namespace MxCaptcha.Options;

public class MxCaptchaOptions
{

    public int CodeLength { get; set; } = 5;

    public bool UseNumbers { get; set; } = true;

    public bool UseLetters { get; set; } = true;

    public bool CaseSensitive { get; set; } = false;

    public int ImageWidth { get; set; } = 200;

    public int ImageHeight { get; set; } = 60;

    public int NoiseLevel { get; set; } = 3;

    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(2);
    public CaptchaImageProvider ImageProvider { get; set; } =
        CaptchaImageProvider.ImageSharp;




    // Font
    public string FontFamily { get; set; } = "DejaVu Sans";
    public float FontSize { get; set; } = 32f;

    // Text & Security
    public bool UseRandomTextColor { get; set; } = true;
    public bool UseRandomRotation { get; set; } = true;
    public float MaxRotationDegrees { get; set; } = 15f;

    public float CharacterSpacing { get; set; } = 30f;

    public bool UseWaveDistortion { get; set; } = true;
    public float WaveAmplitude { get; set; } = 3.0f;
    public float WaveFrequency { get; set; } = 12.0f;


}
