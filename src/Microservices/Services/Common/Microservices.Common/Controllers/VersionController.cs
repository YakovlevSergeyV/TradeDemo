namespace Microservices.Common.Controllers
{
    using System.Net;
    using global::Infrastructure.Common.Providers;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [ApiController]
    public class VersionController : Controller
    {
        private IVersionProvider _versionProvider;

        public VersionController(IVersionProvider versionProvider)
        {
            _versionProvider = versionProvider;
        }


        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        public IActionResult GetVersion()
        {
            return Ok(_versionProvider.GetVersion());
        }
    }
}
