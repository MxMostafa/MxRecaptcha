using System;
using System.Threading.Tasks;
using MxCaptcha.Abstractions;
using MxCaptcha.Models;
using MxCaptcha.Options;

namespace MxCaptcha.Services
{
    public class MxCaptchaService : IMxCaptchaService
    {
        private readonly MxCaptchaOptions _options;
        private readonly ICaptchaStore _store;
        private readonly ICaptchaCodeGenerator _codeGenerator;
        private readonly ICaptchaImageGenerator _imageGenerator;

        public MxCaptchaService(
            MxCaptchaOptions options,
            ICaptchaStore store,
            ICaptchaCodeGenerator codeGenerator,
            ICaptchaImageGenerator imageGenerator)
        {
            _options = options ?? new MxCaptchaOptions();
            _store = store;
            _codeGenerator = codeGenerator;
            _imageGenerator = imageGenerator;
        }

        public async Task<MxCaptchaResult> GenerateAsync()
        {
            var code = _codeGenerator.GenerateCode(_options);
            var captchaId = Guid.NewGuid().ToString("N");
            var expireAt = DateTime.UtcNow.Add(_options.Expiration);

            await _store.SetAsync(captchaId, code, expireAt).ConfigureAwait(false);
            var imageBytes = _imageGenerator.Generate(code, _options);

            return new MxCaptchaResult
            {
                Id = captchaId,
                Base64Image = "data:image/png;base64," + Convert.ToBase64String(imageBytes),
                ExpireAt = expireAt
            };
        }

        public async Task<bool> ValidateAsync(string captchaId, string userInput)
        {
            if (string.IsNullOrWhiteSpace(captchaId) || string.IsNullOrWhiteSpace(userInput))
            {
                return false;
            }

            var storedCode = await _store.GetAsync(captchaId).ConfigureAwait(false);
            if (storedCode == null)
            {
                return false;
            }

            var isValid = _options.CaseSensitive
                ? storedCode == userInput
                : string.Equals(storedCode, userInput, StringComparison.OrdinalIgnoreCase);

            if (isValid)
            {
                await _store.RemoveAsync(captchaId).ConfigureAwait(false);
            }

            return isValid;
        }

        public async Task RefreshAsync(string captchaId)
        {
            if (string.IsNullOrWhiteSpace(captchaId))
            {
                return;
            }

            var newCode = _codeGenerator.GenerateCode(_options);
            var expireAt = DateTime.UtcNow.Add(_options.Expiration);
            await _store.SetAsync(captchaId, newCode, expireAt).ConfigureAwait(false);
        }
    }
}
