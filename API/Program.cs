using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = CreateHostBuilder(args).Build();

            var config = app.Services.GetRequiredService<IConfiguration>();

            foreach (var c in config.AsEnumerable())
            {
                Console.WriteLine(c.Key + " = " + c.Value);
            }

            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    config.AddUserSecrets(appAssembly, true);
                }

                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

            builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>().UseSerilog(); });

            return builder;
        }
    }
}