using RemoteExecution.Core.Executors;
using RemoteExecution.Core.Schedulers;

namespace RemoteExecution.Core.Endpoints
{
	public class ServerEndpointConfig : IServerEndpointConfig
	{
		public ServerEndpointConfig()
		{
			MaxConnections = 128;
			RemoteExecutorFactory = new RemoteExecutorFactory();
			TaskScheduler = new AsyncTaskScheduler();
		}

		#region IServerEndpointConfig Members

		public IRemoteExecutorFactory RemoteExecutorFactory { get; set; }
		public ITaskScheduler TaskScheduler { get; set; }
		public int MaxConnections { get; set; }

		#endregion
	}
}