using MxCaptcha.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxCaptcha.Abstractions;

public interface ICaptchaImageGenerator
{
    byte[] Generate(string code, MxCaptchaOptions options);
}
