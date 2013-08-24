using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Executors
{
	public interface IRemoteExecutorFactory
	{
		IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher);
		IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel);
	}
}