using MxCaptcha.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MxCaptcha.Abstractions;

public interface ICaptchaCodeGenerator
{
    string GenerateCode(MxCaptchaOptions options);
}
