using System;
using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints.Adapters
{
	public interface IEndpointAdapter : IDisposable
	{
		void StartListening();

		Func<IOperationDispatcher> DispatcherCreator { set; }
		Func<INetworkConnection, bool> NewConnectionHandler { set; }
		Action<INetworkConnection> ClosedConnectionHandler { set; }

		IEnumerable<INetworkConnection> ActiveConnections { get; }
	}
}