using Orp.Core.Azure.Function.Template.Core.Interfaces;
using Orp.Core.Azure.Function.Template.Core.Options;
using ProcessManager.Messages.Base;
using System;
using System.Threading.Tasks;

namespace Orp.Core.Azure.Function.Template.Core.Handlers
{
    public class MessageHandler : IHandleMessage<ASB_TaskMessage>
    {
        private readonly HandlerOptions _options;

        public MessageHandler(HandlerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task HandleAsync(ASB_TaskMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
