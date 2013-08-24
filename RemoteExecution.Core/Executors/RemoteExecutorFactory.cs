using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Executors
{
	public class RemoteExecutorFactory : IRemoteExecutorFactory
	{
		#region IRemoteExecutorFactory Members

		public IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher)
		{
			return new RemoteExecutor(channel, dispatcher);
		}

		public IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel)
		{
			return new BroadcastRemoteExecutor(channel);
		}

		#endregion
	}
}