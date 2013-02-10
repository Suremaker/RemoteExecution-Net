using System;
using Lidgren.Network;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Serialization;

namespace RemoteExecution.Networking
{
    public class RemoteOperationManager : NetworkManager
    {
        private static readonly MessageSerializer _serializer = new MessageSerializer();
        private static readonly Action<IWriteEndpoint> NO_ACTION = connection => { };
        private readonly IOperationDispatcher _dispatcher;
        public Action<IWriteEndpoint> OnClosedConnection { get; set; }
        public Action<IWriteEndpoint> OnNewConnection { get; set; }

        protected RemoteOperationManager(NetPeer netPeer, IOperationDispatcher dispatcher)
            : base(netPeer)
        {
            _dispatcher = dispatcher;
            OnNewConnection = NO_ACTION;
            OnClosedConnection = NO_ACTION;
        }

        protected LindgrenWriteEndpoint GetEndpoint(NetConnection connection)
        {
            return new LindgrenWriteEndpoint(Peer, connection);
        }

        protected override void HandleClosedConnection(NetConnection connection)
        {
            OnClosedConnection(GetEndpoint(connection));
        }

        protected override void HandleData(NetIncomingMessage message)
        {
            IMessage msg = _serializer.Deserialize(message.ReadBytes(message.LengthBytes));
            _dispatcher.Dispatch(msg, GetEndpoint(message.SenderConnection));
        }

        protected override void HandleNewConnection(NetConnection connection)
        {
            OnNewConnection(GetEndpoint(connection));
        }
    }
}
