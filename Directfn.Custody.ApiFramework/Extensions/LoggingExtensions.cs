using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Directfn.Custody.ApiFramework.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddDirectfnCustodyLogging(this WebApplicationBuilder builder)
        {
            string applicationName = builder.Configuration["Application:Name"] ?? builder.Environment.ApplicationName;

            string logPath = Path.Combine(AppContext.BaseDirectory, "Logs", $"{applicationName}-.log");

            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] CorrelationId={CorrelationId} {Message:lj}{NewLine}{Exception}").WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] CorrelationId={CorrelationId} {Message:lj}{NewLine}{Exception}").CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }
    }
}