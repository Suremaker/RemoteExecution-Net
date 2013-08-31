using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	public interface IClientConfig
	{
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		ITaskScheduler TaskScheduler { get; }
	}
}