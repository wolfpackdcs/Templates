using Microsoft.Azure.WebJobs;
using Ninject;
using Orp.Core.Logging.Interfaces;
using Orp.Core.Logging.LoggerFactories;
using System;
using Wolfpack.MultiTenant.Core;

namespace Orp.Core.Azure.Function.Template.Functions
{
    public class TimerFunction : FunctionBase
    {
        public TimerFunction(IKernel kernel) : base(kernel) { }

        [FunctionName("DefaultTimerFunction")]
        public void Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, Binder binder)
        {
            foreach (TenantContext tenantContext in GetAllTenants())
            {
                ILog logger = null;

                try
                {
                    logger = GetService<TenantLoggerFactory>(tenantContext.TenantScope).CreateLogger();

                    logger.Information("Running HelloWorldFunction");
                }
                catch (Exception ex)
                {
                    logger?.Error(ex, $"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
