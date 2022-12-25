using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Common.Logging
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
           (context, configuration) =>
           {
               string rootPath = context.HostingEnvironment.ContentRootPath;
               if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
               {
                   rootPath = AppDomain.CurrentDomain.BaseDirectory;
               }

               configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.RollingFile(Path.Combine($"{rootPath}", "logs", "log-{Date}.txt"))
                    .ReadFrom.Configuration(context.Configuration);

           };
    }
}
