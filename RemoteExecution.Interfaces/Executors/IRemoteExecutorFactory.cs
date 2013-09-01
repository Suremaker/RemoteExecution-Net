using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Executors
{
	/// <summary>
	/// Remote executor factory interface allowing to create remote executors.
	/// </summary>
	public interface IRemoteExecutorFactory
	{
		/// <summary>
		/// Creates broadcast remote executor for given broadcast channel.
		/// </summary>
		/// <param name="channel">Broadcast channel.</param>
		/// <returns>Broadcast executor.</returns>
		IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel);
		/// <summary>
		/// Creates remote executor for given channel, using given message dispatcher for receiving operation responses.
		/// </summary>
		/// <param name="channel">Duplex channel.</param>
		/// <param name="dispatcher">Message dispatcher used for receiving operation responses.</param>
		/// <returns>Executor.</returns>
		IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher);
	}
}