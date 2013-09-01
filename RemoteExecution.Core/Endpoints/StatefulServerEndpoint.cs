using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Stateful server endpoint class dedicated for connections requiring own operation dispatcher with dedicated operation handlers.
	/// </summary>
	public abstract class StatefulServerEndpoint : ServerEndpoint
	{
		/// <summary>
		/// Creates stateful server endpoint.
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="config">Server configuration.</param>
		protected StatefulServerEndpoint(IServerConnectionListener listener, IServerConfig config)
			: base(listener, config)
		{
			OnConnectionInitialize += InitializeConnection;
		}

		/// <summary>
		/// Creates stateful server endpoint.
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="config">Server configuration.</param>
		protected StatefulServerEndpoint(string listenerUri, IServerConfig config)
			: base(listenerUri, config)
		{
			OnConnectionInitialize += InitializeConnection;
		}

		/// <summary>
		/// Retrieves operation dispatcher for newly opened connection.
		/// This implementation is always returning new instance of operation dispatcher.
		/// </summary>
		/// <returns>New operation dispatcher instance.</returns>
		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return new OperationDispatcher();
		}

		/// <summary>
		/// Initializes newly opened connection. The implementation can register operation handlers in operation dispatcher assigned to given connection.
		/// </summary>
		/// <param name="connection">Newly opened connection.</param>
		protected abstract void InitializeConnection(IRemoteConnection connection);
	}
}