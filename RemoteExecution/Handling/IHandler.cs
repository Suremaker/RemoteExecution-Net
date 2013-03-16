using RemoteExecution.Messaging;

namespace RemoteExecution.Handling
{
    public interface IHandler
    {
        string Id { get; }
        void Handle(IMessage msg, IMessageChannel messageChannel);
    }
}