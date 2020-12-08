using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Ninject;
using Orp.Core.Azure.Function.Template.Core.Handlers;
using ProcessManager.Messages.Base;
using System.Threading.Tasks;

namespace Orp.Core.Azure.Function.Template.Functions
{
    public class ServiceBusFunction : FunctionBase
    {
        public ServiceBusFunction(IKernel kernel) : base(kernel) { }

        [FunctionName("DefaultListenerFunction")]
        public async Task Run([ServiceBusTrigger("topic-name", "subscription-name", Connection = "ServiceBusConnection")] Message msg,
                               Binder binder)
        {
            Message result = await RunAsMessageHandlerProcessManagerTaskAsync<ASB_TaskMessage, MessageHandler>(msg, binder);
        }
    }
}
