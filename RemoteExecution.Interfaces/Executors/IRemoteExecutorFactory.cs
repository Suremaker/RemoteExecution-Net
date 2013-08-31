using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Executors
{
	public interface IRemoteExecutorFactory
	{
		IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel);
		IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher);
	}
}