using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var launchSettings = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json");
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(launchSettings, optional: true, reloadOnChange: true);
                })
                .Build()
                .Run();
        }
    }
}