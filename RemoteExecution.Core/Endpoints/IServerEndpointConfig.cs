using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Endpoints
{
	public interface IServerEndpointConfig
	{
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		ITaskScheduler TaskScheduler { get; }
		int MaxConnections { get; }
	}
}