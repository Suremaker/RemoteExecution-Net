using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Endpoints
{
	public interface IServerEndpointConfig
	{
		int MaxConnections { get; }
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		ITaskScheduler TaskScheduler { get; }
	}
}