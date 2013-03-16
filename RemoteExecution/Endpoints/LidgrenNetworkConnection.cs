using Lidgren.Network;
using RemoteExecution.Dispatching;
using RemoteExecution.Messaging;
using RemoteExecution.Serialization;

namespace RemoteExecution.Endpoints
{
	internal class LidgrenNetworkConnection : IConfigurableNetworkConnection
	{
		private static readonly MessageSerializer _serializer = new MessageSerializer();
		private readonly NetConnection _connection;

		public LidgrenNetworkConnection(NetConnection connection, IOperationDispatcher operationDispatcher)
		{
			_connection = connection;
			OperationDispatcher = operationDispatcher;
		}

		public void Dispose()
		{
			_connection.Disconnect("Connection disposed");
		}

		public bool IsOpen { get { return _connection.Status == NetConnectionStatus.Connected; } }
		public IOperationDispatcher OperationDispatcher { get; set; }

		public void HandleIncomingMessage(NetIncomingMessage message)
		{
			OperationDispatcher.Dispatch(_serializer.Deserialize(message.ReadBytes(message.LengthBytes)), this);
		}

		public void Send(IMessage message)
		{
			if (!IsOpen)
				throw new NotConnectedException("Network connection is not opened.");
			_connection.Peer.SendMessage(CreateOutgoingMessage(message), _connection, NetDeliveryMethod.ReliableUnordered, 0);
		}

		private NetOutgoingMessage CreateOutgoingMessage(IMessage message)
		{
			byte[] content = _serializer.Serialize(message);
			NetOutgoingMessage msg = _connection.Peer.CreateMessage(content.Length);
			msg.Write(content);
			return msg;
		}
	}
}