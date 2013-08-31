using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Config
{
	public interface IClientConfig
	{
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		ITaskScheduler TaskScheduler { get; }
	}
}