using System;
using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints.Adapters
{
	public interface IEndpointAdapter : IDisposable
	{
		IEnumerable<INetworkConnection> ActiveConnections { get; }
		Action<INetworkConnection> ClosedConnectionHandler { set; }
		Func<IOperationDispatcher> DispatcherCreator { set; }
		Action<INetworkConnection> NewConnectionHandler { set; }
		void StartListening();
	}
}