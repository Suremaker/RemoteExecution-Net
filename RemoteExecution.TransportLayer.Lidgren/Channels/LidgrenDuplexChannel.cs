using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Lidgren duplex channel class allowing to send and receive messages.
	/// </summary>
	public class LidgrenDuplexChannel : DuplexChannel
	{
		private static readonly IEnumerable<NetConnectionStatus> _validConnectionStatus = new[] { NetConnectionStatus.Connected, NetConnectionStatus.RespondedConnect };

		/// <summary>
		/// Returns true if channel is opened, otherwise false.
		/// </summary>
		public override bool IsOpen
		{
			get { return Connection != null && _validConnectionStatus.Contains(Connection.Status); }
		}

		/// <summary>
		/// Lidgren net connection associated to channel.
		/// </summary>
		protected NetConnection Connection { get; set; }

		/// <summary>
		/// Creates channel instance with specified message serializer.
		/// </summary>
		/// <param name="serializer">Message serializer.</param>
		protected LidgrenDuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

		/// <summary>
		/// Creates channel instance with specified net connection and message serializer.
		/// </summary>
		/// <param name="connection">Lidgren net connection.</param>
		/// <param name="serializer">Message serializer.</param>
		public LidgrenDuplexChannel(NetConnection connection, IMessageSerializer serializer)
			: this(serializer)
		{
			Connection = connection;
		}

		/// <summary>
		/// Handles incoming message in lidgren format.
		/// </summary>
		/// <param name="message">Message to handle.</param>
		public void HandleIncomingMessage(NetIncomingMessage message)
		{
			OnReceive(message.ReadBytes(message.LengthBytes));
		}

		/// <summary>
		/// Handles lidgren connection close event (fires Closed event).
		/// </summary>
		public void OnConnectionClose()
		{
			FireChannelClosed();
		}

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
		protected override void Close()
		{
			if (Connection != null && IsOpen)
				Connection.Disconnect("Channel closed");
		}

		/// <summary>
		/// Sends data through channel.
		/// </summary>
		/// <param name="data">Data to send.</param>
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
	}
}
