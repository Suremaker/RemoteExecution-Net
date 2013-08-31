using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	public class ClientConfig : IClientConfig
	{
		public ClientConfig()
		{
			RemoteExecutorFactory = DefaultConfig.RemoteExecutorFactory;
			TaskScheduler = DefaultConfig.TaskScheduler;
		}

		#region IClientConfig Members

		public IRemoteExecutorFactory RemoteExecutorFactory { get; set; }
		public ITaskScheduler TaskScheduler { get; set; }

		#endregion
	}
}