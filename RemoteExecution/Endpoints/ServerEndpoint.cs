using Lidgren.Network;
using RemoteExecution.Messaging;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : LidgrenEndpoint, IServerEndpoint
	{
		public void StartListening()
		{
			Start();
		}

		public IBroadcastChannel BroadcastChannel { get; private set; }

		protected ServerEndpoint(string applicationId, int maxConnections, ushort port)
			: base(new NetServer(new NetPeerConfiguration(applicationId) { MaximumConnections = maxConnections, Port = port }))
		{
			BroadcastChannel = new LindgrenBroadcastChannel((NetServer)Peer);
		}
	}
}