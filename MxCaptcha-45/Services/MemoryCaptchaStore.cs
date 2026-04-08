using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MxCaptcha.Abstractions;

namespace MxCaptcha.Services
{
    public class MemoryCaptchaStore : ICaptchaStore
    {
        private readonly ConcurrentDictionary<string, CaptchaEntry> _store = new ConcurrentDictionary<string, CaptchaEntry>();

        public Task SetAsync(string id, string code, DateTime expireAt)
        {
            _store[id] = new CaptchaEntry(code, expireAt);
            return Task.FromResult(0);
        }

        public Task<string> GetAsync(string id)
        {
            CaptchaEntry entry;
            if (!_store.TryGetValue(id, out entry))
            {
                return Task.FromResult<string>(null);
            }

            if (entry.ExpireAt <= DateTime.UtcNow)
            {
                CaptchaEntry removed;
                _store.TryRemove(id, out removed);
                return Task.FromResult<string>(null);
            }

            return Task.FromResult(entry.Code);
        }

        public Task RemoveAsync(string id)
        {
            CaptchaEntry removed;
            _store.TryRemove(id, out removed);
            return Task.FromResult(0);
        }

        private sealed class CaptchaEntry
        {
            public CaptchaEntry(string code, DateTime expireAt)
            {
                Code = code;
                ExpireAt = expireAt;
            }

            public string Code { get; private set; }

            public DateTime ExpireAt { get; private set; }
        }
    }
}
