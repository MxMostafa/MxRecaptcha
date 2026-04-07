namespace MxCaptcha.Abstractions;

public interface ICaptchaStore
{
    Task SetAsync(string id, string code, DateTime expireAt);

    Task<string?> GetAsync(string id);

    Task RemoveAsync(string id);
}
