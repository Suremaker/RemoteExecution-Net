using RemoteExecution.Config;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Stateless server endpoint dedicated for connections that can share same operation dispatcher with stateless operation handlers.
	/// </summary>
	public class StatelessServerEndpoint : ServerEndpoint
	{
		private readonly IOperationDispatcher _dispatcher;

		/// <summary>
		/// Creates stateless server endpoint with default server configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
		public StatelessServerEndpoint(IServerConnectionListener listener, IOperationDispatcher dispatcher)
			: this(listener, dispatcher, new ServerConfig()) { }

		/// <summary>
		/// Creates stateless server endpoint with default server configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
		public StatelessServerEndpoint(string listenerUri, IOperationDispatcher dispatcher)
			: this(listenerUri, dispatcher, new ServerConfig()) { }

		/// <summary>
		/// Creates stateless server endpoint.
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
		/// <param name="config">Server configuration.</param>
		public StatelessServerEndpoint(IServerConnectionListener listener, IOperationDispatcher dispatcher, IServerConfig config)
			: base(listener, config)
		{
			_dispatcher = dispatcher;
		}

		/// <summary>
		/// Creates stateless server endpoint.
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="dispatcher">Operation dispatcher that would be used for all connections.</param>
		/// <param name="config">Server configuration.</param>
		public StatelessServerEndpoint(string listenerUri, IOperationDispatcher dispatcher, IServerConfig config)
			: base(listenerUri, config)
		{
			_dispatcher = dispatcher;
		}

		/// <summary>
		/// Retrieves operation dispatcher for newly opened connection.
		/// This implementation returns the same operation dispatcher, specified in constructor.
		/// </summary>
		/// <returns>Operation dispatcher for new connection.</returns>
		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _dispatcher;
		}
	}
}