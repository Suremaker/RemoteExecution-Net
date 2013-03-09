using System.Collections.Generic;
using Lidgren.Network;
using RemoteExecution.Dispatching;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : LidgrenEndpoint, IServerEndpoint
	{
		private readonly IDictionary<NetConnection, LidgrenNetworkConnection> _connections = new Dictionary<NetConnection, LidgrenNetworkConnection>();
		protected abstract bool HandleNewConnection(INetworkConnection connection);

		protected virtual void OnConnectionClose(INetworkConnection connection) { }

		public void StartListening()
		{
			Start();
		}

		protected ServerEndpoint(string applicationId, int maxConnections, ushort port)
			: base(new NetServer(new NetPeerConfiguration(applicationId) { MaximumConnections = maxConnections, Port = port }))
		{
		}

		protected override void HandleClosedConnection(NetConnection connection)
		{
			LidgrenNetworkConnection conn;
			if (!_connections.TryGetValue(connection, out conn))
				return;
			_connections.Remove(connection);
			OnConnectionClose(conn);
		}

		protected override void HandleData(NetIncomingMessage message)
		{
			_connections[message.SenderConnection].HandleIncomingMessage(message);
		}

		protected override void HandleNewConnection(NetConnection connection)
		{
			var conn = new LidgrenNetworkConnection(connection, new OperationDispatcher());
			if (HandleNewConnection(conn))
				_connections.Add(connection, conn);
			else 
				conn.Dispose();
		}
	}
}