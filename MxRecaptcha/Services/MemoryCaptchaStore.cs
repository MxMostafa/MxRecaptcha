using Microsoft.Extensions.Caching.Memory;
using MxCaptcha.Abstractions;


namespace MxCaptcha.Services;

internal class MemoryCaptchaStore : ICaptchaStore
{
    private readonly IMemoryCache _cache;

    public MemoryCaptchaStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string?> GetAsync(string id)
    {
        if (!_cache.TryGetValue(id, out string? storedCode))
            return Task.FromResult(storedCode);

        _cache.Remove(id);

        return Task.FromResult(storedCode);
    }

    public Task RemoveAsync(string id)
    {
        _cache.Remove(id);
        return Task.CompletedTask;
    }

    public Task SetAsync(string id, string code, DateTime expireAt)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = expireAt
        };

        _cache.Set(id, code, options);

        return Task.CompletedTask;
    }
}
