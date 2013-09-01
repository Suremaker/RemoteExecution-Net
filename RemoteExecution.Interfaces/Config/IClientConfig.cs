using RemoteExecution.Executors;
using RemoteExecution.Schedulers;

namespace RemoteExecution.Config
{
	/// <summary>
	/// Client configuration interface allowing to configure client dependencies.
	/// </summary>
	public interface IClientConfig
	{
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