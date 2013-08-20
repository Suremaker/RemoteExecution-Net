using System;
using System.Collections.Generic;
using RemoteExecution.Core.Connections;

namespace RemoteExecution.Core.Endpoints
{
	/// <summary>
	/// Interface for server endpoint allowing to listen for incoming connections.
	/// </summary>
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

		/// <summary>
		/// Fires when new connection is opened, it is fully configured and ready to use.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionOpened;

		/// <summary>
		/// Fires when connection has been closed and is no longer on active connections list.
		/// Event is fired in non-blocking way.
		/// </summary>
		event Action<IRemoteConnection> ConnectionClosed;
	}
}
