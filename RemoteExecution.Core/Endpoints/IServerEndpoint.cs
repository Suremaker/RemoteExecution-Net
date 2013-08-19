using System;
using System.Collections.Generic;
using RemoteExecution.Core.Connections;

namespace RemoteExecution.Core.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		IEnumerable<IRemoteConnection> ActiveConnections { get; }

		/// <summary>
		/// Returns true if listener is actively listening for incoming connections.
		/// </summary>
		bool IsListening { get; }

		/// <summary>
		/// Starts listening for incoming connections.
		/// </summary>
		void StartListening();
	}
}
