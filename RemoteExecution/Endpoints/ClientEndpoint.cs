using System.IO;
using System.Linq;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Dispatching;

namespace RemoteExecution.Endpoints
{
	public class ClientEndpoint : LidgrenEndpoint, IClientEndpoint
	{
		private readonly IOperationDispatcher _operationDispatcher;

		public ClientEndpoint(string applicationId)
			: this(applicationId, new OperationDispatcher())
		{
		}

		public ClientEndpoint(string applicationId, IOperationDispatcher operationDispatcher)
			: base(new NetClient(new NetPeerConfiguration(applicationId)))
		{
			_operationDispatcher = operationDispatcher;
		}

		public INetworkConnection Connection
		{
			get
			{
				var connection = Connections.SingleOrDefault();
				if (connection == null)
					throw new NotConnectedException("Network connection is not opened.");
				return connection;
			}
		}

		public INetworkConnection ConnectTo(string host, ushort port)
		{
			Start();
			OpenConnection(host, port);
			return Connection;
		}

		private void OpenConnection(string host, ushort port)
		{
			NetConnection conn = Peer.Connect(host, port);
			while (conn.Status != NetConnectionStatus.Connected || !Connections.Any())
			{
				if (conn.Status == NetConnectionStatus.Disconnected)
					throw new IOException("Connection closed.");

				Thread.Sleep(150);
			}
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			connection.OperationDispatcher = _operationDispatcher;
			return true;
		}
	}
}