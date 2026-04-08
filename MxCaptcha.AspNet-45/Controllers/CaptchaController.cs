using System.Threading.Tasks;
using System.Web.Http;
using MxCaptcha.AspNet45.Models;

namespace MxCaptcha.AspNet45.Controllers
{
    [RoutePrefix("captcha")]
    public class CaptchaController : ApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Generate()
        {
            var result = await CaptchaRuntime.Service.GenerateAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IHttpActionResult> Verify([FromBody] CaptchaVerifyRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            var isValid = await CaptchaRuntime.Service
                .ValidateAsync(request.Id, request.Code)
                .ConfigureAwait(false);

            return Ok(new { success = isValid });
        }
    }
}
