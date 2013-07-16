using RemoteExecution.Channels;
using RemoteExecution.Messages;

namespace RemoteExecution.Handlers
{
    public interface IHandler
    {
        string Id { get; }
		void Handle(IMessage msg, IMessageChannel messageChannel);
    }
}