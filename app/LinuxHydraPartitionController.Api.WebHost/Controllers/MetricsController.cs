using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using LinuxHydraPartitionController.Api.WebHost.Models;

namespace LinuxHydraPartitionController.Api.WebHost
{
    [ApiController]
    [Route("metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<Metrics> _logger;

        public MetricsController(ILogger<Metrics> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(MachineMetrics), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var metrics = new Metrics(_logger);
            MachineMetrics machineMetrics = new MachineMetrics();
            try
            {
                machineMetrics = metrics.GetMetrics();
                return new JsonResult(machineMetrics);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: GetMetrics method failed --> {ex.Message} ==> {ex.StackTrace} ==> {ex.TargetSite}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
