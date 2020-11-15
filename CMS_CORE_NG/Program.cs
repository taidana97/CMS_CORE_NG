using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using LoggingService;
using Microsoft.Extensions.DependencyInjection;
using DataService;
using FunctionalService;

namespace CMS_CORE_NG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            #region Option

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var dpContext = services.GetRequiredService<DataProtectionKeysContext>();
                    var functionSvc = services.GetRequiredService<IFunctionalSvc>();

                    DbContextInitializer.Initialize(dpContext, context, functionSvc).Wait();
                }
                catch (Exception ex)
                {
                    Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }


            #endregion

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    #region
                    webBuilder.UseSerilog((hostingContext, loggingConfiguration) => loggingConfiguration
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", "CMS_CORE_NG")
                        .Enrich.WithProperty("MachineName", Environment.MachineName)
                        .Enrich.WithProperty("CurrentManagedThreadId", Environment.CurrentManagedThreadId)
                        .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                        .Enrich.WithProperty("Version", Environment.Version)
                        .Enrich.WithProperty("UserName", Environment.UserName)
                        .Enrich.WithProperty("ProcessId", Process.GetCurrentProcess().Id)
                        .Enrich.WithProperty("ProcessName", Process.GetCurrentProcess().ProcessName)
                        .WriteTo.Console(theme: CustomConsoleTheme.VisualStudioMacLight)
                        .WriteTo.File(formatter: new CustomTextFormatter(),
                            path: Path.Combine(hostingContext.HostingEnvironment.ContentRootPath + $"{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}",
                            $"cms_core_ng_{DateTime.Now:yyyyMMdd}.txt"))
                        .ReadFrom.Configuration(hostingContext.Configuration));
                    #endregion

                    webBuilder.UseStartup<Startup>();
                });
    }
}
