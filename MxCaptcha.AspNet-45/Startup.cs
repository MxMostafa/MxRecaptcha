using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;

namespace MxCaptcha.AspNet45
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
