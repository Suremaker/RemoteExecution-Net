using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lidgren.Network;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Processing;

namespace RemoteExecution.Endpoints
{
	public abstract class LidgrenEndpoint : IDisposable
	{
		private readonly IDictionary<NetConnection, LidgrenNetworkConnection> _connections = new ConcurrentDictionary<NetConnection, LidgrenNetworkConnection>();
		protected readonly NetPeer Peer;
		private MessageLoop _messageLoop;

		protected IEnumerable<INetworkConnection> Connections { get { return _connections.Values; } }

		protected void Start()
		{
			_messageLoop = new MessageLoop(Peer, HandleMessage);
			Peer.Start();
		}

		protected LidgrenEndpoint(NetPeer peer)
		{
			Peer = peer;
		}

		public void Dispose()
		{
			Peer.Shutdown("Endpoint disposed");
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}

		protected abstract IOperationDispatcher GetDispatcherForNewConnection();
		protected virtual bool OnNewConnection(INetworkConnection connection) { return true; }
		protected virtual void OnConnectionClose(INetworkConnection connection) { }

		private void HandleClosedConnection(NetConnection connection)
		{
			LidgrenNetworkConnection conn;
			if (!_connections.TryGetValue(connection, out conn))
				return;
			_connections.Remove(connection);
			OnConnectionClose(conn);
			conn.Dispose();
		}

		private void HandleNewConnection(NetConnection connection)
		{
			var conn = new LidgrenNetworkConnection(connection, GetDispatcherForNewConnection());
			if (OnNewConnection(conn))
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