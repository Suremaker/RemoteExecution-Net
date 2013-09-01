using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	/// <summary>
	/// Server configuration interface allowing to configure server dependencies as well as specify maximum number of allowed connections.
	/// </summary>
	public interface IServerConfig
	{
		/// <summary>
		/// Returns maximum number of allowed connections that should be handled by server.
		/// </summary>
		int MaxConnections { get; }
		/// <summary>
		/// Returns remote executor factory that should be used by client.
		/// </summary>
		IRemoteExecutorFactory RemoteExecutorFactory { get; }
		/// <summary>
		/// Returns task scheduler that should be used by client.
		/// </summary>
		ITaskScheduler TaskScheduler { get; }
	}
}