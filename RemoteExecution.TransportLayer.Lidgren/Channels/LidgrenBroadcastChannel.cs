using Lidgren.Network;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.TransportLayer.Lidgren.Channels
{
	public class LidgrenBroadcastChannel : OutputChannel, IBroadcastChannel
	{
		private readonly NetServer _netServer;

		public LidgrenBroadcastChannel(NetServer netServer, IMessageSerializer serializer)
			: base(serializer)
		{
			_netServer = netServer;
		}

		#region IBroadcastChannel Members

		public override bool IsOpen
		{
			get { return _netServer.Status == NetPeerStatus.Running; }
		}

		public int ReceiverCount { get { return _netServer.ConnectionsCount; } }

		#endregion

		protected override void Close()
		{
		}

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