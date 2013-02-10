using RemoteExecution.Endpoints;

namespace RemoteExecution.Handling
{
    public interface IHandler
    {
        string Id { get; }
        void Handle(IMessage msg, IWriteEndpoint writeEndpoint);
    }
}