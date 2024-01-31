
namespace Microservices.Common.Controllers
{
    using System.Net;
    using Microservices.Logging.Abstract;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [ApiController]
    public class LoggingController : Controller
    {
        private ILoggerManager _loggerManager;

        public LoggingController(ILoggerManager loggerManager)
        {
            _loggerManager = loggerManager;
        }

        [Route("Level")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        public IActionResult GetLevel()
        {
            return Ok(_loggerManager.LoggerLevel.ToString());
        }

        /// <summary>
        /// Включить уровень логирования Trace
        /// </summary>
        [Route("LevelTrace")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelTrace()
        {
            _loggerManager.LoggerLevel = LogLevel.Trace;
            return NoContent();
        }

        /// <summary>
        /// Включить уровень логирования Debug
        /// </summary>
        [Route("LevelDebug")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelDebug()
        {
            _loggerManager.LoggerLevel = LogLevel.Debug;
            return NoContent();
        }

        /// <summary>
        /// Включить уровень логирования Information
        /// </summary>
        [Route("LevelInformation")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelInformation()
        {
            _loggerManager.LoggerLevel = LogLevel.Information;
            return NoContent();
        }

        /// <summary>
        /// Включить уровень логирования Warning
        /// </summary>
        [Route("LevelWarning")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelWarning()
        {
            _loggerManager.LoggerLevel = LogLevel.Warning;
            return NoContent();
        }

        /// <summary>
        /// Включить уровень логирования Error
        /// </summary>
        [Route("LevelError")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelError()
        {
            _loggerManager.LoggerLevel = LogLevel.Error;
            return NoContent();
        }

        /// <summary>
        /// Включить уровень логирования Critical
        /// </summary>
        [Route("LevelCritical")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchInLevelCritical()
        {
            _loggerManager.LoggerLevel = LogLevel.Critical;
            return NoContent();
        }

        /// <summary>
        /// Выключить логирование
        /// </summary>
        [Route("switchoff")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public IActionResult SwitchOff()
        {
            _loggerManager.LoggerLevel = LogLevel.None;

            return NoContent();
        }
    }
}
