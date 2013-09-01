using System;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Generic server endpoint class allowing to configure server endpoint behavior in flexible way.
	/// </summary>
	public class GenericServerEndpoint : ServerEndpoint
	{
		private Func<IOperationDispatcher> _operationDispatcherCreator;
		/// <summary>
		/// Creates generic server endpoint with default server configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="operationDispatcherCreator">Method used to create operation dispatcher for new connection.</param>
		/// <param name="connectionInitializer">Method used to initialize new connection. If null, no action is taken.</param>
		public GenericServerEndpoint(string listenerUri, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: this(listenerUri, new ServerConfig(), operationDispatcherCreator, connectionInitializer) { }
		/// <summary>
		/// Creates generic server endpoint.
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="config">Server configuration.</param>
		/// <param name="operationDispatcherCreator">Method used to create operation dispatcher for new connection.</param>
		/// <param name="connectionInitializer">Method used to initialize new connection. If null, no action is taken.</param>
		public GenericServerEndpoint(string listenerUri, IServerConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listenerUri, config)
		{
			Initialize(operationDispatcherCreator, connectionInitializer);
		}
		/// <summary>
		/// Creates generic server endpoint with default server configuration (<see cref="DefaultConfig"/>).
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="operationDispatcherCreator">Method used to create operation dispatcher for new connection.</param>
		/// <param name="connectionInitializer">Method used to initialize new connection. If null, no action is taken.</param>
		public GenericServerEndpoint(IServerConnectionListener listener, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: this(listener, new ServerConfig(), operationDispatcherCreator, connectionInitializer) { }

		/// <summary>
		/// Creates generic server endpoint.
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="config">Server configuration.</param>
		/// <param name="operationDispatcherCreator">Method used to create operation dispatcher for new connection.</param>
		/// <param name="connectionInitializer">Method used to initialize new connection. If null, no action is taken.</param>
		public GenericServerEndpoint(IServerConnectionListener listener, IServerConfig config, Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer = null)
			: base(listener, config)
		{
			Initialize(operationDispatcherCreator, connectionInitializer);
		}

		/// <summary>
		/// Retrieves operation dispatcher for newly opened connection.
		/// </summary>
		/// <returns>Operation dispatcher for new connection.</returns>
		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _operationDispatcherCreator.Invoke();
		}

		private void Initialize(Func<IOperationDispatcher> operationDispatcherCreator, Action<IServerEndpoint, IRemoteConnection> connectionInitializer)
		{
			_operationDispatcherCreator = operationDispatcherCreator;
			if (connectionInitializer != null)
				OnConnectionInitialize += connection => connectionInitializer(this, connection);
		}
	}
}