using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	/// <summary>
	/// Default configuration used by both server and client classes.
	/// </summary>
	public static class DefaultConfig
	{
		/// <summary>
		/// Default remote executor factory.
		/// </summary>
		public static IRemoteExecutorFactory RemoteExecutorFactory { get; private set; }

		/// <summary>
		/// Default task scheduler.
		/// </summary>
		public static ITaskScheduler TaskScheduler { get; private set; }

		static DefaultConfig()
		{
			RemoteExecutorFactory = new RemoteExecutorFactory();
			TaskScheduler = new AsyncTaskScheduler();
		}
	}
}