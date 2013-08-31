using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	public static class DefaultConfig
	{
		public static IRemoteExecutorFactory RemoteExecutorFactory { get; private set; }

		public static ITaskScheduler TaskScheduler { get; private set; }

		static DefaultConfig()
		{
			RemoteExecutorFactory = new RemoteExecutorFactory();
			TaskScheduler = new AsyncTaskScheduler();
		}
	}
}