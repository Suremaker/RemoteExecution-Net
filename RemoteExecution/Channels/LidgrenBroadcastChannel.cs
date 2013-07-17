using Lidgren.Network;
using RemoteExecution.Messages;
using RemoteExecution.Serialization;

namespace RemoteExecution.Channels
{
	internal class LidgrenBroadcastChannel : IBroadcastChannel
	{
		private static readonly MessageSerializer _serializer = new MessageSerializer();
		private readonly NetServer _netServer;

		public LidgrenBroadcastChannel(NetServer netServer)
		{
			_netServer = netServer;
		}

		#region IBroadcastChannel Members

		public bool IsOpen { get { return _netServer.Status == NetPeerStatus.Running; } }

		public void Send(IMessage message)
		{
			if (ReceiverCount > 0)
				_netServer.SendToAll(CreateOutgoingMessage(message), NetDeliveryMethod.ReliableUnordered);
		}

		public int ReceiverCount { get { return _netServer.ConnectionsCount; } }

		#endregion

		private NetOutgoingMessage CreateOutgoingMessage(IMessage message)
		{
			byte[] content = _serializer.Serialize(message);
			NetOutgoingMessage msg = _netServer.CreateMessage(content.Length);
			msg.Write(content);
			return msg;
		}
	}
}