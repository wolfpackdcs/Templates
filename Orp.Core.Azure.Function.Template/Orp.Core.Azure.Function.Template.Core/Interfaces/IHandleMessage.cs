using ProcessManager.Messages.Base;
using System.Threading.Tasks;

namespace Orp.Core.Azure.Function.Template.Core.Interfaces
{
    public interface IHandleMessage<in TMessage> where TMessage : ASB_TaskMessage
    {
        Task HandleAsync(TMessage message);
    }
}
