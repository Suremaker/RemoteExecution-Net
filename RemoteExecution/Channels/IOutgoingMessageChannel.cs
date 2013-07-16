using RemoteExecution.Messages;

namespace RemoteExecution.Channels
{
	public interface IOutgoingMessageChannel
	{
		void Send(IMessage message);
	}
}