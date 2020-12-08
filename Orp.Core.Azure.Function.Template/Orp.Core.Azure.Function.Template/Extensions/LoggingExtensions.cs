using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orp.Core.Azure.Function.Template.Configuration;
using Orp.Core.Logging.Adapters.Core.Log4Net;
using Orp.Core.Logging.Interfaces;
using System.IO;

namespace Orp.Core.Azure.Function.Template.Extensions
{
    internal static class LoggingExtensions
    {
        internal static IServiceCollection AddLog4NetLogging(this IServiceCollection services,
            ConfigurationHelper configurationHelper)
        {
            services.AddSingleton<ILog, Log4NetCoreWrapper>();

            return services.AddLogging(config =>
            {
                config
                    .AddFilter("Microsoft", LogLevel.None)
                    .AddFilter("System", LogLevel.Debug)
                    .AddLog4Net(Path.Combine(configurationHelper.AppBaseDirectory, $"log4net.{configurationHelper.CurrentEnvironment}.config"));
            });
        }
    }
}
