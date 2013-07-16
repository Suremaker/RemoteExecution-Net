using System;
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
		private MessageLoop _messageLoop;
		protected readonly NetPeer Peer;

		public Action<INetworkConnection> ClosedConnectionHandler { set; private get; }
		public IEnumerable<INetworkConnection> ActiveConnections { get { return Peer.Connections.Select(ToNetworkConnection); } }

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

			Peer.Shutdown("Endpoint disposed");
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}

		private void HandleClosedConnection(NetConnection netConnection)
		{
			var connection = ToNetworkConnection(netConnection);

			ClosedConnectionHandler.Invoke(connection);
			connection.Dispose();
		}

		private void HandleNewConnection(NetConnection netConnection)
		{
			var connection = new LidgrenNetworkConnection(netConnection, DispatcherCreator.Invoke());
			netConnection.Tag = connection;
			NewConnectionHandler.Invoke(connection);
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
			ToNetworkConnection(message.SenderConnection).Channel.HandleIncomingMessage(message);
		}

		private LidgrenNetworkConnection ToNetworkConnection(NetConnection netConnection)
		{
			return (LidgrenNetworkConnection)netConnection.Tag;
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