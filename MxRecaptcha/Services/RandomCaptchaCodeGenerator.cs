using MxCaptcha.Abstractions;
using MxCaptcha.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MxCaptcha.Services;

public class RandomCaptchaCodeGenerator : ICaptchaCodeGenerator
{
    private const string Numbers = "0123456789";
    private const string Letters = "abcdefghijklmnopqrstuvwxyz";

    public string GenerateCode(MxCaptchaOptions options)
    {
        var characterPool = new StringBuilder();

        if (options.UseNumbers)
            characterPool.Append(Numbers);

        if (options.UseLetters)
        {
            characterPool.Append(Letters);

            if (options.CaseSensitive)
                characterPool.Append(Letters.ToUpper());
        }

        if (characterPool.Length == 0)
            throw new InvalidOperationException("No characters configured for captcha generation.");

        string pool = characterPool.ToString();

        var result = new char[options.CodeLength];

        for (int i = 0; i < result.Length; i++)
        {
            int index = RandomNumberGenerator.GetInt32(pool.Length);
            result[i] = pool[index];
        }

        return new string(result);
    }
}
