using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Executors
{
	public class RemoteExecutorFactory : IRemoteExecutorFactory
	{
		public IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher)
		{
			return new RemoteExecutor(channel, dispatcher);
		}
	}
}