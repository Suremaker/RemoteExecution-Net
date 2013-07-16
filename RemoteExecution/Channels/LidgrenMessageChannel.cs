using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using RemoteExecution.Connections;
using RemoteExecution.Messages;
using RemoteExecution.Serialization;

namespace RemoteExecution.Channels
{
	internal class LidgrenMessageChannel : IMessageChannel, IDisposable
	{
		private static readonly IEnumerable<NetConnectionStatus> _validConnectionStatus = new[] { NetConnectionStatus.Connected, NetConnectionStatus.RespondedConnect };
		private static readonly MessageSerializer _serializer = new MessageSerializer();
		private readonly NetConnection _connection;

		public LidgrenMessageChannel(NetConnection connection)
		{
			_connection = connection;
		}

		public bool IsOpen { get { return _validConnectionStatus.Contains(_connection.Status); } }

		public event Action<IMessage> Received;

		public void Send(IMessage message)
		{
			if (!IsOpen)
				throw new NotConnectedException("Network connection is not opened.");
			_connection.Peer.SendMessage(CreateOutgoingMessage(message), _connection, NetDeliveryMethod.ReliableUnordered, 0);
		}

		public void HandleIncomingMessage(NetIncomingMessage message)
		{
			Received.Invoke(_serializer.Deserialize(message.ReadBytes(message.LengthBytes)));
		}

		private NetOutgoingMessage CreateOutgoingMessage(IMessage message)
		{
			byte[] content = _serializer.Serialize(message);
			NetOutgoingMessage msg = _connection.Peer.CreateMessage(content.Length);
			msg.Write(content);
			return msg;
		}

		public void Dispose()
		{
			_connection.Disconnect("Connection disposed");
		}
	}
}