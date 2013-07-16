using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Processing;

namespace RemoteExecution.Endpoints.Adapters
{
	public class LidgrenEndpointAdapter : IEndpointAdapter
	{
		private readonly IDictionary<NetConnection, LidgrenNetworkConnection> _connections = new ConcurrentDictionary<NetConnection, LidgrenNetworkConnection>();
		private MessageLoop _messageLoop;
		protected readonly NetPeer Peer;

		public Action<INetworkConnection> ClosedConnectionHandler { set; private get; }
		public IEnumerable<INetworkConnection> ActiveConnections { get { return _connections.Values; } }

		public void StartListening()
		{
			_messageLoop = new MessageLoop(Peer, HandleMessage);
			Peer.Start();
		}

		public Func<IOperationDispatcher> DispatcherCreator { set; private get; }
		public Func<INetworkConnection, bool> NewConnectionHandler { set; private get; }

		public LidgrenEndpointAdapter(NetPeer peer)
		{
			Peer = peer;

			ClosedConnectionHandler = connection => { };
			DispatcherCreator = () => new OperationDispatcher();
			NewConnectionHandler = connection => true;
		}

		public void Dispose()
		{
			foreach (var connection in _connections.Values.ToArray())
				connection.Dispose();

			Peer.Shutdown("Endpoint disposed");
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}

		private void HandleClosedConnection(NetConnection connection)
		{
			LidgrenNetworkConnection conn;
			if (!_connections.TryGetValue(connection, out conn))
				return;
			_connections.Remove(connection);
			ClosedConnectionHandler.Invoke(conn);
			conn.Dispose();
		}

		private void HandleNewConnection(NetConnection connection)
		{
			var conn = new LidgrenNetworkConnection(connection, DispatcherCreator.Invoke());
			if (NewConnectionHandler.Invoke(conn))
				_connections.Add(connection, conn);
			else
				conn.Dispose();
		}

		private void HandleMessage(NetIncomingMessage msg)
		{
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.Data:
					HandleData(msg);
					break;
				case NetIncomingMessageType.StatusChanged:
					HandleStatusChange(msg);
					break;
			}
		}

		private void HandleData(NetIncomingMessage message)
		{
			_connections[message.SenderConnection].Channel.HandleIncomingMessage(message);
		}

		private void HandleStatusChange(NetIncomingMessage msg)
		{
			switch ((NetConnectionStatus)msg.ReadByte())
			{
				case NetConnectionStatus.Connected:
					HandleNewConnection(msg.SenderConnection);
					break;
				case NetConnectionStatus.Disconnected:
					HandleClosedConnection(msg.SenderConnection);
					break;
			}
		}
	}
}