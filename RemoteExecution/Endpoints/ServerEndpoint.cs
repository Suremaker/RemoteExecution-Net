using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Executors;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : LidgrenEndpoint, IServerEndpoint
	{
		public void StartListening()
		{
			Start();
		}

		public IBroadcastRemoteExecutor BroadcastRemoteExecutor { get; private set; }

		protected IBroadcastChannel BroadcastChannel { get; private set; }

		protected ServerEndpoint(string applicationId, int maxConnections, ushort port)
			: base(new NetServer(new NetPeerConfiguration(applicationId) { MaximumConnections = maxConnections, Port = port }))
		{
			BroadcastChannel = new LidgrenBroadcastChannel((NetServer)Peer);
			BroadcastRemoteExecutor = new BroadcastRemoteExecutor(BroadcastChannel);
		}
	}
}