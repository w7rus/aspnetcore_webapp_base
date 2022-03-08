using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("Application starting...");

            var app = CreateHostBuilder(args).Build();

            var env = app.Services.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                var config = app.Services.GetRequiredService<IConfiguration>();

                foreach (var c in config.AsEnumerable())
                {
                    Console.WriteLine(c.Key + " = " + c.Value);
                }
            }

            try
            {
                await app.RunAsync();
                Log.Information("Application stopping...");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "An unhandled exception occured during bootstrapping!");
            }
            finally
            {
                Log.Information("Flushing logs...");
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Log.Information("Add JSON configurations...");
                    config.AddJsonFile(
                        Path.Combine(hostingContext.HostingEnvironment.ContentRootPath,
                            $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json"), optional: true,
                        reloadOnChange: true);
                    config.AddJsonFile(
                        Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, $"appsettings.Local.json"),
                        optional: true, reloadOnChange: true);

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        Log.Information("Add user secrets...");
                        var appAssembly =
                            Assembly.Load(new AssemblyName(hostingContext.HostingEnvironment.ApplicationName));
                        config.AddUserSecrets(appAssembly, true);
                    }

                    Log.Information("Add environment variables...");
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        Log.Information("Add command line args...");
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>().UseSerilog(); });
        }
    }
}