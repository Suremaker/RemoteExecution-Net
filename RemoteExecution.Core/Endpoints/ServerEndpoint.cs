using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	/// <summary>
	/// Abstract implementation of server endpoint, containing common logic for maintaining connections.
	/// </summary>
	public abstract class ServerEndpoint : IServerEndpoint
	{
		private readonly IServerListener _listener;
		private readonly IServerEndpointConfig _config;
		private readonly ConcurrentDictionary<Guid, IRemoteConnection> _connections = new ConcurrentDictionary<Guid, IRemoteConnection>();

		protected ServerEndpoint(IServerListener listener, IServerEndpointConfig config)
		{
			_listener = listener;
			_config = config;
			_listener.OnChannelOpen += OnChannelOpen;
		}

		private void OnChannelOpen(IDuplexChannel channel)
		{
			var connection = new RemoteConnection(channel, _config.RemoteExecutorFactory, GetOperationDispatcher(), _config.TaskScheduler);
			var channelId = channel.Id;

			connection.Closed += () => HandleConnectionClose(channelId);

			if (OnConnectionInitialize != null)
				OnConnectionInitialize(connection);

			if (!_connections.TryAdd(channelId, connection))
				channel.Dispose();

			HandleConnectionOpen(connection);
		}

		private void HandleConnectionOpen(RemoteConnection connection)
		{
			if (ConnectionOpened != null)
				_config.TaskScheduler.Execute(() => FireConnectionOpened(connection));
		}

		private void HandleConnectionClose(Guid channelId)
		{
			IRemoteConnection connection;
			if (_connections.TryRemove(channelId, out connection) && (ConnectionClosed != null))
				_config.TaskScheduler.Execute(() => FireConnectionClosed(connection));
		}

		private void FireConnectionOpened(IRemoteConnection connection)
		{
			if (ConnectionOpened != null)
				ConnectionOpened(connection);
		}

		private void FireConnectionClosed(IRemoteConnection connection)
		{
			if (ConnectionClosed != null)
				ConnectionClosed(connection);
		}

		/// <summary>
		/// Event fired when new connection is created. It can be used to configure connection (like registering dedicated handlers in OperationDispatcher).
		/// Event is fired in blocking way, so it's handlers should be lightweight.
		/// To perform blocking heavier (blocking) operations / initialization on newly opened connection, please use ConnectionOpened event.
		/// </summary>
		protected event Action<IRemoteConnection> OnConnectionInitialize;

		/// <summary>
		/// Fires when new connection is opened, it is fully configured and ready to use.
		/// Event is fired in non-blocking way.
		/// </summary>
		public event Action<IRemoteConnection> ConnectionOpened;

		/// <summary>
		/// Fires when connection has been closed and is no longer on active connections list.
		/// Event is fired in non-blocking way.
		/// </summary>
		public event Action<IRemoteConnection> ConnectionClosed;

		/// <summary>
		/// Retrieves operation dispatcher for newly opened connection.
		/// </summary>
		/// <returns>Operation dispatcher for new connection.</returns>
		protected abstract IOperationDispatcher GetOperationDispatcher();

		/// <summary>
		/// Closes all active connections and disposes listener.
		/// </summary>
		public void Dispose()
		{
			foreach (var connection in ActiveConnections)
				connection.Dispose();
			_listener.Dispose();
		}

		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		public IEnumerable<IRemoteConnection> ActiveConnections { get { return _connections.Values; } }

		/// <summary>
		/// Returns true if listener is actively listening for incoming connections.
		/// </summary>
		public bool IsListening { get { return _listener.IsListening; } }

		/// <summary>
		/// Starts listening for incoming connections.
		/// </summary>
		public void StartListening()
		{
			_listener.StartListening();
		}
	}
}