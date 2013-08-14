using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Channels
{
	public interface IOutputChannel : IChannel
	{
		void Send(IMessage message);
	}
}