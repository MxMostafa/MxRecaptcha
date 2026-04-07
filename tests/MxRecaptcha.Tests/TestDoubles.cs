using Microsoft.Extensions.Caching.Distributed;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;

namespace MxRecaptcha.Tests;

internal sealed class FakeCaptchaStore : ICaptchaStore
{
    public readonly Dictionary<string, (string Code, DateTime ExpireAt)> Values = new();
    public int RemoveCallCount { get; private set; }

    public Task SetAsync(string id, string code, DateTime expireAt)
    {
        Values[id] = (code, expireAt);
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync(string id)
    {
        if (Values.TryGetValue(id, out var value))
        {
            return Task.FromResult<string?>(value.Code);
        }

        return Task.FromResult<string?>(null);
    }

    public Task RemoveAsync(string id)
    {
        RemoveCallCount++;
        Values.Remove(id);
        return Task.CompletedTask;
    }
}

internal sealed class FakeCaptchaCodeGenerator : ICaptchaCodeGenerator
{
    private readonly string _code;

    public FakeCaptchaCodeGenerator(string code)
    {
        _code = code;
    }

    public int GenerateCallCount { get; private set; }

    public string GenerateCode(MxCaptchaOptions options)
    {
        GenerateCallCount++;
        return _code;
    }
}

internal sealed class FakeCaptchaImageGenerator : ICaptchaImageGenerator
{
    private readonly byte[] _bytes;

    public FakeCaptchaImageGenerator(byte[] bytes)
    {
        _bytes = bytes;
    }

    public string? LastCode { get; private set; }
    public MxCaptchaOptions? LastOptions { get; private set; }

    public byte[] Generate(string code, MxCaptchaOptions options)
    {
        LastCode = code;
        LastOptions = options;
        return _bytes;
    }
}

internal sealed class FakeDistributedCache : IDistributedCache
{
    public readonly Dictionary<string, string> Values = new();
    public readonly Dictionary<string, DistributedCacheEntryOptions> EntryOptions = new();

    public byte[]? Get(string key)
    {
        return Values.TryGetValue(key, out var value) ? System.Text.Encoding.UTF8.GetBytes(value) : null;
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        return Task.FromResult(Get(key));
    }

    public void Refresh(string key)
    {
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        Values.Remove(key);
        EntryOptions.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        Values[key] = System.Text.Encoding.UTF8.GetString(value);
        EntryOptions[key] = options;
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }
}
