using System;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints
{
	public class GenericServerEndpoint : ServerEndpoint
	{
		private static readonly Action<INetworkConnection> _noAction = connection => { };
		private readonly Func<IOperationDispatcher> _dispatcherCreator;
		private readonly Action<INetworkConnection> _onConnectionClosed;
		private readonly Action<INetworkConnection> _onNewConnection;

		public GenericServerEndpoint(ServerEndpointConfig config, Func<IOperationDispatcher> dispatcherCreator, Action<INetworkConnection> onNewConnection = null, Action<INetworkConnection> onConnectionClosed = null)
			: base(config)
		{
			_dispatcherCreator = dispatcherCreator;
			_onNewConnection = onNewConnection ?? _noAction;
			_onConnectionClosed = onConnectionClosed ?? _noAction;
		}

		protected sealed override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return _dispatcherCreator();
		}

		protected sealed override void OnConnectionClose(INetworkConnection connection)
		{
			_onConnectionClosed(connection);
		}

		protected sealed override void OnNewConnection(INetworkConnection connection)
		{
			_onNewConnection(connection);
		}
	}
}