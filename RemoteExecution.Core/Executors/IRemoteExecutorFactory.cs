using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Executors
{
	public interface IRemoteExecutorFactory
	{
		IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel);
		IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher);
	}
}