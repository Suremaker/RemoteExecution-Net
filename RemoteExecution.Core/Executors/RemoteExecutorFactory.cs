using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Executors
{
	/// <summary>
	/// Remote executor factory class allowing to create remote executors.
	/// </summary>
	public class RemoteExecutorFactory : IRemoteExecutorFactory
	{
		#region IRemoteExecutorFactory Members

		/// <summary>
		/// Creates remote executor for given channel, using given message dispatcher for receiving operation responses.
		/// </summary>
		/// <param name="channel">Duplex channel.</param>
		/// <param name="dispatcher">Message dispatcher used for receiving operation responses.</param>
		/// <returns>Executor.</returns>
		public IRemoteExecutor CreateRemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher)
		{
			return new RemoteExecutor(channel, dispatcher);
		}

		/// <summary>
		/// Creates broadcast remote executor for given broadcast channel.
		/// </summary>
		/// <param name="channel">Broadcast channel.</param>
		/// <returns>Broadcast executor.</returns>
		public IBroadcastRemoteExecutor CreateBroadcastRemoteExecutor(IBroadcastChannel channel)
		{
			return new BroadcastRemoteExecutor(channel);
		}

		#endregion
	}
}