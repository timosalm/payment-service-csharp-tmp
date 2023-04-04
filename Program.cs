using System;
using KubeServiceBinding;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Logging;

namespace PaymentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DotnetServiceBinding serviceBinding = new DotnetServiceBinding();
                Dictionary<string, string> configServerServiceBinding = serviceBinding.GetBindings("config-server");
                Environment.SetEnvironmentVariable("Cloud.Config.Uri", configServerServiceBinding["uri"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Service Binding not found");
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .AddConfigServer()
                .ConfigureLogging((context, builder) => builder.AddDynamicConsole())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
