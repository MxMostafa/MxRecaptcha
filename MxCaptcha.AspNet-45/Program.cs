using System;
using Microsoft.Owin.Hosting;

namespace MxCaptcha.AspNet45
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var baseAddress = "http://localhost:5005";

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine("MxCaptcha.AspNet-45 started at " + baseAddress);
                Console.WriteLine("GET  /captcha");
                Console.WriteLine("POST /captcha/verify");
                Console.WriteLine("Press Enter to stop.");
                Console.ReadLine();
            }
        }
    }
}
