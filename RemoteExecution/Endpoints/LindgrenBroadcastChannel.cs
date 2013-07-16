using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Messages;
using RemoteExecution.Serialization;

namespace RemoteExecution.Endpoints
{
	internal class LindgrenBroadcastChannel : IBroadcastChannel
	{
		private static readonly MessageSerializer _serializer = new MessageSerializer();
		private readonly NetServer _netServer;

		public LindgrenBroadcastChannel(NetServer netServer)
		{
			_netServer = netServer;
		}

		public void Send(IMessage message)
		{
			if (ConnectionCount > 0)
				_netServer.SendToAll(CreateOutgoingMessage(message), NetDeliveryMethod.ReliableUnordered);
		}

		private NetOutgoingMessage CreateOutgoingMessage(IMessage message)
		{
			byte[] content = _serializer.Serialize(message);
			NetOutgoingMessage msg = _netServer.CreateMessage(content.Length);
			msg.Write(content);
			return msg;
		}

		public int ConnectionCount { get { return _netServer.ConnectionsCount; } }
	}
}