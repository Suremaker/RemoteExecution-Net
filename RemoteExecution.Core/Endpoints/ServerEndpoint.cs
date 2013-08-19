using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	public abstract class ServerEndpoint : IServerEndpoint
	{
		private readonly IServerListener _listener;
		private readonly IServerEndpointConfig _config;
		private readonly IDictionary<Guid, IRemoteConnection> _connections = new ConcurrentDictionary<Guid, IRemoteConnection>();

		protected ServerEndpoint(IServerListener listener, IServerEndpointConfig config)
		{
			_listener = listener;
			_config = config;
			_listener.OnChannelOpen += OnChannelOpen;
		}

		private void OnChannelOpen(IDuplexChannel channel)
		{
			var connection = new RemoteConnection(channel, _config.RemoteExecutorFactory, GetOperationDispatcher(), _config.TaskScheduler);

			connection.Closed += () => _connections.Remove(channel.Id);
			_connections.Add(channel.Id, connection);
		}

		protected abstract IOperationDispatcher GetOperationDispatcher();

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