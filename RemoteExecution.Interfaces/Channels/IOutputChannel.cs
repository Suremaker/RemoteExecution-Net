using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Channels
{
	public interface IOutputChannel : IChannel
	{
		void Send(IMessage message);
	}
}