using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Channels
{
	public interface IOutgoingMessageChannel
	{
		bool IsOpen { get; }
		void Send(IMessage message);
	}
}