using System.Collections.Generic;
using System.Threading.Tasks;
using Lidgren.Network;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints.Processing;

namespace RemoteExecution.Endpoints
{
	public abstract class LidgrenEndpoint : INetworkEndpoint
	{
		private readonly IDictionary<NetConnection, LidgrenNetworkConnection> _connections = new Dictionary<NetConnection, LidgrenNetworkConnection>();
		protected readonly NetPeer Peer;
		private MessageLoop _messageLoop;

		protected IEnumerable<INetworkConnection> Connections { get { return _connections.Values; } }

		protected void Start()
		{
			_messageLoop = new MessageLoop(this);
			Peer.Start();
		}

		protected LidgrenEndpoint(NetPeer peer)
		{
			Peer = peer;
			_messageLoop = new MessageLoop(this);
		}

		public void Dispose()
		{
			Peer.Shutdown("Endpoint disposed");
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}

		public bool ProcessMessage()
		{
			var msg = Peer.ReadMessage();
			if (msg == null)
				return false;
			Task.Factory.StartNew(() => HandleMessage(msg));
			return true;
		}

		protected abstract bool HandleNewConnection(IConfigurableNetworkConnection connection);
		protected virtual void OnConnectionClose(INetworkConnection connection) { }

		private void HandleClosedConnection(NetConnection connection)
		{
			LidgrenNetworkConnection conn;
			if (!_connections.TryGetValue(connection, out conn))
				return;
			_connections.Remove(connection);
			OnConnectionClose(conn);
			conn.OperationDispatcher.DispatchAbortResponses(conn, "Network connection has been closed.");
			conn.Dispose();
		}

		private void HandleNewConnection(NetConnection connection)
		{
			var conn = new LidgrenNetworkConnection(connection, new OperationDispatcher());
			if (HandleNewConnection(conn))
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
			_connections[message.SenderConnection].HandleIncomingMessage(message);
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