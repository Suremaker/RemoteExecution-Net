using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Adapters;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : IServerEndpoint
	{
		private readonly LidgrenServerEndpointAdapter _endpointAdapter;

		protected ServerEndpoint(ServerEndpointConfig config)
		{
			_endpointAdapter = new LidgrenServerEndpointAdapter(config)
			{
				DispatcherCreator = GetDispatcherForNewConnection,
				NewConnectionHandler = OnNewConnection,
				ClosedConnectionHandler = OnConnectionClose
			};

			BroadcastRemoteExecutor = new BroadcastRemoteExecutor(_endpointAdapter.BroadcastChannel);
		}

		#region IServerEndpoint Members

		public void StartListening()
		{
			_endpointAdapter.StartListening();
		}

		public IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; private set; }
		public IEnumerable<INetworkConnection> ActiveConnections { get { return _endpointAdapter.ActiveConnections; } }

		public void Dispose()
		{
			_endpointAdapter.Dispose();
		}

		#endregion

		protected abstract IOperationDispatcher GetDispatcherForNewConnection();
		protected virtual void OnConnectionClose(INetworkConnection connection) { }
		protected virtual void OnNewConnection(INetworkConnection connection) { }
	}
}