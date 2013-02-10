using RemoteExecution.Endpoints;
using RemoteExecution.Handling;

namespace RemoteExecution.Dispatching
{
    public interface IOperationDispatcher
    {
        void AddHandler(IHandler handler);
        void Dispatch(IMessage msg, IWriteEndpoint writeEndpoint);
        void RegisterFor<TInterface>(TInterface handler);
        void RemoveHandler(IHandler handler);
        void Unregister<TInterface>();
    }
}