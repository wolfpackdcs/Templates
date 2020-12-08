using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using Orp.Core.Azure.Function.Template.Core.Interfaces;
using Orp.Core.Logging.Interfaces;
using Orp.Core.Logging.LoggerFactories;
using ProcessManager.Messages.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wolfpack.AzureServiceBus;
using Wolfpack.MultiTenant.Core;

namespace Orp.Core.Azure.Function.Template.Functions
{
    /// <summary>
    /// Serves as the base class for all functions, 
    /// facilitating access to the Ninject kernel.
    /// </summary>
    public abstract class FunctionBase
    {
        private readonly IKernel _kernel;

        public FunctionBase(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        /// <summary>
        /// Retrieves a specified service from the Ninject container. 
        /// If the service type is not configured in the container, an exception is thrown. 
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <param name="tenantScope">The tenant and scope for which the service needs to be retrieved. 
        /// The provided value needs to conform to the following format: "{tenantId}-{scopeId}"</param>
        /// <returns>The implemention of the requested service.</returns>
        protected T GetService<T>(string tenantScope, IBinder binder = null) where T : class
        {
            var service = _kernel.Get<T>(new Parameter("TenantScope", tenantScope, true),
                                         new ConstructorArgument("binder", binder, true));
            if (service is null)
            {
                throw new Exception($"No service of type '{typeof(T)}' is configured for tenant and scope combination '{tenantScope}'.");
            }

            return service;
        }

        /// <summary>
        /// Retrieves all the configured tenant contexts.
        /// </summary>
        /// <returns>An enumerable of all the configured tenant contexts.</returns>
        protected IEnumerable<TenantContext> GetAllTenants()
        {
            return _kernel.GetAll<TenantContext>();
        }

        protected Task<Message> RunAsMessageHandlerProcessManagerTaskAsync<TMessage, TMessageHandler>(Message message, IBinder binder = null) where TMessage : ASB_TaskMessage
                                                                                                                                              where TMessageHandler : class, IHandleMessage<TMessage>
        => RunAsProcessManagerTaskAsync<TMessage>(message, (msg, tenantScope) =>
        {
            var handler = _kernel.Get<TMessageHandler>(GetNinjectParameter(tenantScope),
                                                       new ConstructorArgument("binder", binder, true));
            return handler.HandleAsync(msg);
        });

        private async Task<Message> RunAsProcessManagerTaskAsync<TMessage>(Message message, Func<TMessage, string, Task> action) where TMessage : ASB_TaskMessage
        {
            TMessage msg = JsonConvert.DeserializeObject<TMessage>(Encoding.UTF8.GetString(message.Body));

            ILog logger = null;

            try
            {
                TenantContext tenantContext = new TenantContext(msg.TenantId, msg.ScopeId);

                logger = _kernel.Get<TenantLoggerFactory>(GetNinjectParameter(tenantContext.TenantScope)).CreateLogger();

                await action(msg, GetTenantScope(msg));

                return CreateTaskResultMessage(msg);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, ex.Message);
                return CreateTaskResultMessage(msg, ex.Message);
            }
        }

        protected string GetTenantScope<TMessage>(TMessage message) where TMessage : ASB_TaskMessage
       => $"{message.TenantId}-{message.ScopeId}";

        protected Parameter GetNinjectParameter(string tenantScope)
           => new Parameter("TenantScope", tenantScope, true);


        protected Message CreateTaskResultMessage(ASB_TaskMessage message, string errorMessage = null)
        {
            return new ASB_TaskResultMessage()
            {
                ErrorMessage = errorMessage,
                Id = message.Id,
                IsSuccess = errorMessage == null,
                ProcessData = message.ProcessData,
                ScopeId = message.ScopeId,
                TaskId = message.TaskId,
                TenantId = message.TenantId
            }
            .AsMessage(message.TenantId, message.ScopeId);
        }
    }
}
