using System;

namespace MxCaptcha.Models
{
    public class MxCaptchaResult
    {
        public string Id { get; set; }

        public string Base64Image { get; set; }

        public DateTime ExpireAt { get; set; }
    }
}
