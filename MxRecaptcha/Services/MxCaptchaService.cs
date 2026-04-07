using Microsoft.Extensions.Options;
using MxCaptcha.Abstractions;
using MxCaptcha.Models;
using MxCaptcha.Options;


namespace MxCaptcha.Services;

public class MxCaptchaService : IMxCaptchaService
{
    private readonly MxCaptchaOptions _options;
    private readonly ICaptchaStore _store;
    private readonly ICaptchaCodeGenerator _codeGenerator;
    private readonly ICaptchaImageGenerator _imageGenerator;

    public MxCaptchaService(
        IOptions<MxCaptchaOptions> options,
        ICaptchaStore store,
        ICaptchaCodeGenerator codeGenerator,
        ICaptchaImageGenerator imageGenerator)
    {
        _options = options.Value;
        _store = store;
        _codeGenerator = codeGenerator;
        _imageGenerator = imageGenerator;
    }

    public async Task<MxCaptchaResult> GenerateAsync()
    {
        // 1. Generate captcha code
        string code = _codeGenerator.GenerateCode(_options);

        // 2. Create unique ID
        string captchaId = Guid.NewGuid().ToString("N");

        // 3. Store the captcha code
        DateTime expireAt = DateTime.UtcNow.Add(_options.Expiration);
        await _store.SetAsync(captchaId, code, expireAt);

        // 4. Generate image
        byte[] imageBytes = _imageGenerator.Generate(code, _options);

        // 5. Convert to base64 image
        string base64 = Convert.ToBase64String(imageBytes);

        // 6. Build response model
        return new MxCaptchaResult
        {
            Id = captchaId,
            Base64Image = $"data:image/png;base64,{base64}",
            ExpireAt = expireAt
        };
    }

    public async Task<bool> ValidateAsync(string captchaId, string userInput)
    {
        if (string.IsNullOrWhiteSpace(captchaId) || string.IsNullOrWhiteSpace(userInput))
            return false;

        // Load expected code from store
        string? storedCode = await _store.GetAsync(captchaId);

        if (storedCode == null)
            return false;

        // Case sensitivity check
        bool result = _options.CaseSensitive
            ? storedCode == userInput
            : string.Equals(storedCode, userInput, StringComparison.OrdinalIgnoreCase);

        // Remove captcha after verification (on success)
        if (result)
            await _store.RemoveAsync(captchaId);

        return result;
    }

    public async Task RefreshAsync(string captchaId)
    {
        if (string.IsNullOrWhiteSpace(captchaId))
            return;

        // Generate a new code
        string newCode = _codeGenerator.GenerateCode(_options);

        DateTime expireAt = DateTime.UtcNow.Add(_options.Expiration);

        await _store.SetAsync(captchaId, newCode, expireAt);
    }
}

