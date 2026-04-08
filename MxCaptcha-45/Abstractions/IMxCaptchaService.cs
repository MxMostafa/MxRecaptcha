using System.Threading.Tasks;
using MxCaptcha.Models;

namespace MxCaptcha.Abstractions
{
    public interface IMxCaptchaService
    {
        Task<MxCaptchaResult> GenerateAsync();

        Task<bool> ValidateAsync(string captchaId, string userInput);

        Task RefreshAsync(string captchaId);
    }
}
