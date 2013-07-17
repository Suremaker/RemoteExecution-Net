using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Adapters;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public class ClientEndpoint : IClientEndpoint
	{
		private readonly IClientEndpointAdapter _endpointAdapter;
		private INetworkConnection _connection;

		public INetworkConnection Connection
		{
			get
			{
				if (_connection == null)
					throw new NotConnectedException("Network connection is not opened.");
				return _connection;
			}
		}

		public ClientEndpoint(string applicationId)
			: this(applicationId, new OperationDispatcher())
		{
		}

		public ClientEndpoint(string applicationId, IOperationDispatcher operationDispatcher)
			: this(new LidgrenClientEndpointAdapter(applicationId), operationDispatcher)
		{
		}

		public ClientEndpoint(IClientEndpointAdapter endpointAdapter, IOperationDispatcher operationDispatcher)
		{
			_endpointAdapter = endpointAdapter;
			_endpointAdapter.DispatcherCreator = () => operationDispatcher;
			_endpointAdapter.NewConnectionHandler = connection => _connection = connection;
			_endpointAdapter.ClosedConnectionHandler = connection => _connection = null;
		}

		#region IClientEndpoint Members

		public INetworkConnection ConnectTo(string host, ushort port)
		{
			_endpointAdapter.StartListening();
			_endpointAdapter.ConnectTo(host, port);
			return Connection;
		}

		public IRemoteExecutor RemoteExecutor { get { return Connection.RemoteExecutor; } }

		public void Dispose()
		{
			_endpointAdapter.Dispose();
		}

		#endregion
	}
}