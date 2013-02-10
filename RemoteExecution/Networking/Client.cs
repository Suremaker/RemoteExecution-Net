using System.IO;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;

namespace RemoteExecution.Networking
{
    public class Client : RemoteOperationManager
    {
        public IWriteEndpoint Endpoint { get; private set; }

        public Client(string applicationId, IOperationDispatcher dispatcher)
            : base(CreateClient(applicationId), dispatcher)
        {
        }

        private static NetClient CreateClient(string applicationId)
        {
            return new NetClient(new NetPeerConfiguration(applicationId));
        }

        public void Connect(string host, int port)
        {
            Endpoint = new LindgrenWriteEndpoint(Peer, OpenConnection(host, port));
        }

        private NetConnection OpenConnection(string host, int port)
        {
            NetConnection conn = Peer.Connect(host, port);
            while (conn.Status != NetConnectionStatus.Connected && conn.Status != NetConnectionStatus.Disconnected)
                Thread.Sleep(150);
            if (conn.Status == NetConnectionStatus.Disconnected)
                throw new IOException("Connection closed.");
            return conn;
        }
    }
}