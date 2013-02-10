using Lidgren.Network;
using RemoteExecution.Dispatching;

namespace RemoteExecution.Networking
{
    public class Server : RemoteOperationManager
    {
        public Server(string applicationId, IOperationDispatcher dispatcher, int maxConnections, int port)
            : base(CreateServer(applicationId, maxConnections, port), dispatcher)
        {
        }

        private static NetServer CreateServer(string applicationId, int maxConnections, int port)
        {
            return new NetServer(new NetPeerConfiguration(applicationId)
            {
                MaximumConnections = maxConnections,
                Port = port
            });
        }
    }
}