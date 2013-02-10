using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using RemoteExecution.Serialization;

namespace RemoteExecution.Endpoints
{
    public class LindgrenWriteEndpoint : IWriteEndpoint
    {
        private static readonly MessageSerializer _serializer = new MessageSerializer();
        private readonly List<NetConnection> _connection;
        private readonly NetPeer _peer;

        public LindgrenWriteEndpoint(NetPeer peer, params NetConnection[] connection)
        {
            _peer = peer;
            _connection = connection.ToList();
        }

        #region IWriteEndpoint Members

        public void Send(IMessage message)
        {
            _peer.SendMessage(CreateMessage(message), _connection, NetDeliveryMethod.ReliableUnordered, 0);
        }

        #endregion

        private NetOutgoingMessage CreateMessage(IMessage message)
        {
            byte[] content = _serializer.Serialize(message);
            NetOutgoingMessage msg = _peer.CreateMessage(content.Length);
            msg.Write(content);
            return msg;
        }
    }
}
