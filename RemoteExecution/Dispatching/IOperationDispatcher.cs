using RemoteExecution.Handling;
using RemoteExecution.Messaging;

namespace RemoteExecution.Dispatching
{
    public interface IOperationDispatcher
    {
        void AddHandler(IHandler handler);
        void Dispatch(IMessage msg, IMessageSender messageSender);
        void RegisterFor<TInterface>(TInterface handler);
        void RemoveHandler(IHandler handler);
        void Unregister<TInterface>();
    }
}