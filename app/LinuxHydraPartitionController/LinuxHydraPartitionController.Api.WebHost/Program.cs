using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LinuxHydraPartitionController.Api.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder
                        .ConfigureKestrel(serverOptions => serverOptions.Listen(IPAddress.Any, 5000))
                        .UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}