using Microsoft.Extensions.Caching.Distributed;
using MxCaptcha.Abstractions;

namespace MxCaptcha.Services;

public class DistributedCaptchaStore : ICaptchaStore
{
    private readonly IDistributedCache _cache;

    public DistributedCaptchaStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync(string id, string code, DateTime expireAt)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expireAt
        };

        await _cache.SetStringAsync(id, code, options);
    }

    public async Task<string?> GetAsync(string id)
    {
        return await _cache.GetStringAsync(id);
    }

    public async Task RemoveAsync(string id)
    {
        await _cache.RemoveAsync(id);
    }
}
