﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RemoteExecution.Channels;
using RemoteExecution.Config;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Executors;
using RemoteExecution.TransportLayer;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Abstract implementation of server endpoint, containing common logic for maintaining connections.
	/// </summary>
	public abstract class ServerEndpoint : IServerEndpoint
	{
		private readonly IServerConfig _config;
		private readonly IConnectionConfig _connectionConfig;
		private readonly ConcurrentDictionary<Guid, IRemoteConnection> _connections = new ConcurrentDictionary<Guid, IRemoteConnection>();
		private readonly IServerConnectionListener _listener;

		/// <summary>
		/// Fires when connection has been closed and is no longer on active connections list.
		/// Event is fired in non-blocking way.
		/// </summary>
		public event Action<IRemoteConnection> ConnectionClosed;

		/// <summary>
		/// Fires when new connection is opened, it is fully configured and ready to use.
		/// Event is fired in non-blocking way.
		/// </summary>
		public event Action<IRemoteConnection> ConnectionOpened;

		/// <summary>
		/// Creates server endpoint instance.
		/// </summary>
		/// <param name="listenerUri">Listener uri used to create server connection listener.</param>
		/// <param name="config">Server configuration.</param>
		protected ServerEndpoint(string listenerUri, IServerConfig config)
			: this(TransportLayerResolver.CreateConnectionListenerFor(new Uri(listenerUri)), config)
		{
		}

		/// <summary>
		/// Creates server endpoint instance.
		/// </summary>
		/// <param name="listener">Server connection listener used to listen for incoming connections.</param>
		/// <param name="config">Server configuration.</param>
		protected ServerEndpoint(IServerConnectionListener listener, IServerConfig config)
		{
			_listener = listener;
			_config = config;
			_connectionConfig = new ConnectionConfig
			{
				RemoteExecutorFactory = _config.RemoteExecutorFactory,
				TaskScheduler = _config.TaskScheduler
			};
			_listener.OnChannelOpen += OnChannelOpen;
			BroadcastRemoteExecutor = config.RemoteExecutorFactory.CreateBroadcastRemoteExecutor(listener.BroadcastChannel);
		}

		#region IServerEndpoint Members

		/// <summary>
		/// Closes listener and all active connections.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dispose()
		{
			_listener.Dispose();

			foreach (var connection in ActiveConnections)
				connection.Dispose();
		}

		/// <summary>
		/// List of fully configured, active connections.
		/// </summary>
		public IEnumerable<IRemoteConnection> ActiveConnections { get { return _connections.Values; } }

		/// <summary>
		/// Returns true if endpoint is accepting incoming connections.
		/// </summary>
		public bool IsRunning { get { return _listener.IsListening; } }

		/// <summary>
		/// Starts accepting incoming connections.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Start()
		{
			if (IsRunning)
				throw new InvalidOperationException("Server already started.");
			try
			{
				_listener.StartListening();
			}
			catch (Exception ex)
			{
				throw new ServerStartException("Unable to start server: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Returns broadcast executor.
		/// </summary>
		public IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; private set; }

		#endregion

		/// <summary>
		/// Retrieves operation dispatcher for newly opened connection.
		/// </summary>
		/// <returns>Operation dispatcher for new connection.</returns>
		protected abstract IOperationDispatcher GetOperationDispatcher();

		/// <summary>
		/// Event fired when new connection is created. It can be used to configure connection (like registering dedicated handlers in OperationDispatcher).
		/// Event is fired in blocking way, so it's handlers should be lightweight.
		/// To perform blocking heavier (blocking) operations / initialization on newly opened connection, please use ConnectionOpened event.
		/// </summary>
		protected event Action<IRemoteConnection> OnConnectionInitialize;

		private void FireConnectionClosed(IRemoteConnection connection)
		{
			if (ConnectionClosed != null)
				ConnectionClosed(connection);
		}

		private void FireConnectionOpened(IRemoteConnection connection)
		{
			if (ConnectionOpened != null)
				ConnectionOpened(connection);
		}

		private void HandleConnectionClose(Guid channelId)
		{
			IRemoteConnection connection;
			if (_connections.TryRemove(channelId, out connection) && (ConnectionClosed != null))
				_config.TaskScheduler.Execute(() => FireConnectionClosed(connection));
		}

		private void HandleConnectionOpen(RemoteConnection connection)
		{
			if (ConnectionOpened != null)
				_config.TaskScheduler.Execute(() => FireConnectionOpened(connection));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void OnChannelOpen(IDuplexChannel channel)
		{
			if (_connections.Count >= _config.MaxConnections)
			{
				channel.Dispose();
				return;
			}

			var connection = new RemoteConnection(channel, GetOperationDispatcher(), _connectionConfig);
			var channelId = channel.Id;

			connection.Closed += () => HandleConnectionClose(channelId);

			if (OnConnectionInitialize != null)
				OnConnectionInitialize(connection);

			if (!_connections.TryAdd(channelId, connection))
				channel.Dispose();

			HandleConnectionOpen(connection);
		}
	}
}