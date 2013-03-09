using System.IO;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Dispatching;

namespace RemoteExecution.Endpoints
{
	public class ClientEndpoint : LidgrenEndpoint, IClientEndpoint
	{
		private readonly IOperationDispatcher _operationDispatcher;
		private LidgrenNetworkConnection _connection;

		public ClientEndpoint(string applicationId, IOperationDispatcher operationDispatcher)
			: base(new NetClient(new NetPeerConfiguration(applicationId)))
		{
			_operationDispatcher = operationDispatcher;
		}

		public INetworkConnection ConnectTo(string host, ushort port)
		{
			Start();
			return _connection = OpenConnection(host, port);
		}

		private LidgrenNetworkConnection OpenConnection(string host, ushort port)
		{
			NetConnection conn = Peer.Connect(host, port);
			while (conn.Status != NetConnectionStatus.Connected && conn.Status != NetConnectionStatus.Disconnected)
				Thread.Sleep(150);
			if (conn.Status == NetConnectionStatus.Disconnected)
				throw new IOException("Connection closed.");
			return new LidgrenNetworkConnection(conn,_operationDispatcher);
		}


		protected override void HandleData(NetIncomingMessage message)
		{
			_connection.HandleIncomingMessage(message);
		}
	}
}