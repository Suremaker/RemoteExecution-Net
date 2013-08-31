using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Config
{
	public class ServerConfig : IServerConfig
	{
		public ServerConfig()
		{
			MaxConnections = 128;
			RemoteExecutorFactory = DefaultConfig.RemoteExecutorFactory;
			TaskScheduler = DefaultConfig.TaskScheduler;
		}

		#region IServerConfig Members

		public IRemoteExecutorFactory RemoteExecutorFactory { get; set; }
		public ITaskScheduler TaskScheduler { get; set; }
		public int MaxConnections { get; set; }

		#endregion
	}
}