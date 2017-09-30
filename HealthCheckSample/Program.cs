﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HealthCheckSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseHealthChecks("/hc")
                .UseStartup<Startup>()
                .Build();
    }
}
