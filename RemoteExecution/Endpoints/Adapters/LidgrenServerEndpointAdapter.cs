using Lidgren.Network;
using RemoteExecution.Channels;

namespace RemoteExecution.Endpoints.Adapters
{
	internal class LidgrenServerEndpointAdapter : LidgrenEndpointAdapter, IServerEndpointAdapter
	{
		public LidgrenServerEndpointAdapter(string applicationId, int maxConnections, ushort port)
			: base(new NetServer(new NetPeerConfiguration(applicationId) { MaximumConnections = maxConnections, Port = port }))
		{
			BroadcastChannel = new LidgrenBroadcastChannel((NetServer)Peer);
		}

		public IBroadcastChannel BroadcastChannel { get; private set; }
	}
}