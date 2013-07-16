using System.Linq;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Adapters;

namespace RemoteExecution.Endpoints
{
	public class ClientEndpoint : IClientEndpoint
	{
		private readonly IClientEndpointAdapter _endpointAdapter;

		public ClientEndpoint(string applicationId)
			: this(applicationId, new OperationDispatcher())
		{
		}

		public ClientEndpoint(string applicationId, IOperationDispatcher operationDispatcher)
			: this(new LidgrenClientEndpointAdapter(applicationId) { DispatcherCreator = () => operationDispatcher })
		{
		}

		public ClientEndpoint(IClientEndpointAdapter endpointAdapter)
		{
			_endpointAdapter = endpointAdapter;
		}

		public INetworkConnection Connection
		{
			get
			{
				var connection = _endpointAdapter.ActiveConnections.FirstOrDefault();
				if (connection == null)
					throw new NotConnectedException("Network connection is not opened.");
				return connection;
			}
		}

		public INetworkConnection ConnectTo(string host, ushort port)
		{
			_endpointAdapter.StartListening();
			_endpointAdapter.ConnectTo(host, port);
			return Connection;
		}

		public void Dispose()
		{
			_endpointAdapter.Dispose();
		}
	}
}