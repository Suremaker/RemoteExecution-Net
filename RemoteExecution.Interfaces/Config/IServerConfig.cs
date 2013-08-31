using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	public interface IServerConfig
	{
		int MaxConnections { get; }
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		ITaskScheduler TaskScheduler { get; }
	}
}