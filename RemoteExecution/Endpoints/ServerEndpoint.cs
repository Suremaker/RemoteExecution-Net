using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Adapters;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : IServerEndpoint
	{
		private readonly LidgrenServerEndpointAdapter _endpointAdapter;

		public void StartListening()
		{
			_endpointAdapter.StartListening();
		}

		public IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; private set; }

		protected ServerEndpoint(string applicationId, int maxConnections, ushort port)
		{
			_endpointAdapter = new LidgrenServerEndpointAdapter(applicationId, maxConnections, port)
			{
				DispatcherCreator = GetDispatcherForNewConnection,
				NewConnectionHandler = OnNewConnection,
				ClosedConnectionHandler = OnConnectionClose
			};

			BroadcastRemoteExecutor = new BroadcastRemoteExecutor(_endpointAdapter.BroadcastChannel);
		}

		public void Dispose()
		{
			_endpointAdapter.Dispose();
		}

		protected abstract IOperationDispatcher GetDispatcherForNewConnection();
		protected virtual void OnNewConnection(INetworkConnection connection) { }
		protected virtual void OnConnectionClose(INetworkConnection connection) { }
	}
}