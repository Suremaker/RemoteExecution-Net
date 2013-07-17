using System;
using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		void StartListening();
		IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; }

		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		IEnumerable<INetworkConnection> ActiveConnections { get; }
	}
}