using Lidgren.Network;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Lidgren broadcast channel class allowing to send broadcast messages to all opened channels.
	/// It can be used only on server side.
	/// </summary>
	public class LidgrenBroadcastChannel : OutputChannel, IBroadcastChannel
	{
		private readonly NetServer _netServer;

		/// <summary>
		/// Creates channel instance with given lidgren net server and message serializer.
		/// </summary>
		/// <param name="netServer">Lidgren net server.</param>
		/// <param name="serializer">Message serializer.</param>
		public LidgrenBroadcastChannel(NetServer netServer, IMessageSerializer serializer)
			: base(serializer)
		{
			_netServer = netServer;
		}

		#region IBroadcastChannel Members

		/// <summary>
		/// Returns true if channel is opened, otherwise false.
		/// </summary>
		public override bool IsOpen
		{
			get { return _netServer.Status == NetPeerStatus.Running; }
		}

		/// <summary>
		/// Returns count of receivers who will get sent message.
		/// </summary>
		public int ReceiverCount { get { return _netServer.ConnectionsCount; } }

		#endregion

		/// <summary>
		/// This implementation does nothing.
		/// </summary>
		protected override void Close()
		{
		}

		/// <summary>
		/// Sends data through channel.
		/// </summary>
		/// <param name="data">Data to send.</param>
		protected override void SendData(byte[] data)
		{
			if (!IsOpen)
				throw new NotConnectedException("Connection is not opened.");

			if (ReceiverCount > 0)
				_netServer.SendToAll(CreateOutgoingMessage(data), NetDeliveryMethod.ReliableUnordered);
		}

		private NetOutgoingMessage CreateOutgoingMessage(byte[] data)
		{
			var msg = _netServer.CreateMessage(data.Length);
			msg.Write(data);
			return msg;
		}
	}
}