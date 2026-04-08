using System;
using MxCaptcha.Abstractions;
using MxCaptcha.Options;

namespace MxCaptcha.Services
{
    public class RandomCaptchaCodeGenerator : ICaptchaCodeGenerator
    {
        private readonly Random _random = new Random();

        public string GenerateCode(MxCaptchaOptions options)
        {
            var chars = string.Empty;

            if (options.UseNumbers)
            {
                chars += "0123456789";
            }

            if (options.UseLetters)
            {
                chars += options.CaseSensitive
                    ? "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
                    : "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (string.IsNullOrEmpty(chars))
            {
                chars = "0123456789";
            }

            var buffer = new char[options.CodeLength];
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = chars[_random.Next(chars.Length)];
            }

            return new string(buffer);
        }
    }
}
