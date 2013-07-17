using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints.Processing;

namespace RemoteExecution.Endpoints.Adapters
{
	public class LidgrenEndpointAdapter : IEndpointAdapter
	{
		private MessageLoop _messageLoop;
		protected static readonly TimeSpan SynchronizationTimeSpan = TimeSpan.FromMilliseconds(25);
		protected readonly NetPeer Peer;

		public Action<INetworkConnection> ClosedConnectionHandler { set; private get; }
		public IEnumerable<INetworkConnection> ActiveConnections { get { return Peer.Connections.Select(ExtractConnection).Where(conn => conn != null); } }

		public void StartListening()
		{
			_messageLoop = new MessageLoop(Peer, HandleMessage);
			Peer.Start();
		}

		public Func<IOperationDispatcher> DispatcherCreator { set; private get; }
		public Action<INetworkConnection> NewConnectionHandler { set; private get; }

		public LidgrenEndpointAdapter(NetPeer peer)
		{
			Peer = peer;

			ClosedConnectionHandler = connection => { };
			DispatcherCreator = () => new OperationDispatcher();
			NewConnectionHandler = connection => { };
		}

		public void Dispose()
		{
			foreach (var connection in ActiveConnections.ToArray())
				connection.Dispose();

			ShutdownPeer();

			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}

		private void ShutdownPeer()
		{
			Peer.Shutdown("Endpoint disposed");
			while (Peer.Status != NetPeerStatus.NotRunning)
				Thread.Sleep(SynchronizationTimeSpan);
		}

		private void HandleClosedConnection(NetConnection netConnection)
		{
			var connection = ExtractConnectionWithWait(netConnection);

			ClosedConnectionHandler.Invoke(connection);
			connection.Dispose();
		}

		private void HandleNewConnection(NetConnection netConnection)
		{
			lock (netConnection)
			{
				var connection = new LidgrenNetworkConnection(netConnection, DispatcherCreator.Invoke());
				NewConnectionHandler.Invoke(connection);
				netConnection.Tag = connection;
			}
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
			ExtractConnectionWithWait(message.SenderConnection).Channel.HandleIncomingMessage(message);
		}

		private LidgrenNetworkConnection ExtractConnectionWithWait(NetConnection netConnection)
		{
			var connection = ExtractConnection(netConnection);
			if (connection != null)
				return connection;

			lock (netConnection)
				return ExtractConnection(netConnection);
		}

		private LidgrenNetworkConnection ExtractConnection(NetConnection netConnection)
		{
			return netConnection.Tag as LidgrenNetworkConnection;
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