using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using LinuxHydraPartitionController.Api.WebHost.Models;

namespace LinuxHydraPartitionController.Api.WebHost.Controllers
{
    [ApiController]
    [Route("logfile")]
    public class LogfileController : Controller
    {
        private readonly ILogger<Logfile> _logger;
        private readonly IConfiguration Configuration;

        public LogfileController(ILogger<Logfile> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            Configuration = configuration;
        }

        // Get File status by partition id (file size and epoch time stamp)
        [HttpGet("{id:int}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get([FromRoute] int id)
        {
            var logfile = new Logfile(_logger, id, Configuration);
            LogFileProps logfileprops = new LogFileProps();
            try
            {
                logfileprops = logfile.GetLogFileProps(id);
                return new JsonResult(logfileprops);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: GetLogFileProps method failed --> {ex.Message} ==> {ex.StackTrace} ==> {ex.TargetSite}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Download logfile using text/plain ContentType
        [HttpGet("{id:int}/download")]
        public async Task<ActionResult> DownloadFile([FromRoute] int id)
        {
            var logfile = new Logfile(_logger, id, Configuration);
            LogFileProps logfileprops = new LogFileProps();
            try
            {
                logfileprops = logfile.GetLogFileProps(id);
                var bytes = await System.IO.File.ReadAllBytesAsync(logfileprops.logfileName);
                return File(bytes, "text/plain", Path.GetFileName(logfileprops.logfileName));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: DownloadFile method failed --> {ex.Message} ==> {ex.StackTrace} ==> {ex.TargetSite}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

