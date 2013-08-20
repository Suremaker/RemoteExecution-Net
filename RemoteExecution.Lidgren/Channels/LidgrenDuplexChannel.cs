using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Lidgren.Channels
{
	public class LidgrenDuplexChannel : DuplexChannel
	{
		private static readonly IEnumerable<NetConnectionStatus> _validConnectionStatus = new[] { NetConnectionStatus.Connected, NetConnectionStatus.RespondedConnect };
		protected NetConnection Connection { get; set; }

		protected LidgrenDuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

		public LidgrenDuplexChannel(NetConnection connection, IMessageSerializer serializer)
			: this(serializer)
		{
			Connection = connection;
		}

		protected override void Close()
		{
			if (Connection != null)
				Connection.Disconnect("Channel closed");
		}

		public override bool IsOpen
		{
			get { return Connection != null && _validConnectionStatus.Contains(Connection.Status); }
		}

		protected override void SendData(byte[] data)
		{
			if (!IsOpen)
				throw new NotConnectedException("Network connection is not opened.");
			Connection.Peer.SendMessage(CreateOutgoingMessage(data), Connection, NetDeliveryMethod.ReliableUnordered, 0);
		}

		private NetOutgoingMessage CreateOutgoingMessage(byte[] data)
		{
			var msg = Connection.Peer.CreateMessage(data.Length);
			msg.Write(data);
			return msg;
		}

		public void HandleIncomingMessage(NetIncomingMessage message)
		{
			OnReceive(message.ReadBytes(message.LengthBytes));
		}
	}
}
