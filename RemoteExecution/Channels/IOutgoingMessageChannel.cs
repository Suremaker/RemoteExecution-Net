using RemoteExecution.Messages;

namespace RemoteExecution.Channels
{
	public interface IOutgoingMessageChannel
	{
		bool IsOpen { get; }
		void Send(IMessage message);
	}
}