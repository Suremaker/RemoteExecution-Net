using Lidgren.Network;
using RemoteExecution.Channels;

namespace RemoteExecution.Endpoints.Adapters
{
	internal class LidgrenServerEndpointAdapter : LidgrenEndpointAdapter, IServerEndpointAdapter
	{
		public LidgrenServerEndpointAdapter(ServerEndpointConfig config)
			: base(new NetServer(ToNetPeerConfiguration(config)))
		{
			BroadcastChannel = new LidgrenBroadcastChannel((NetServer)Peer);
		}

		private static NetPeerConfiguration ToNetPeerConfiguration(ServerEndpointConfig config)
		{
			return new NetPeerConfiguration(config.ApplicationId)
			{
				MaximumConnections = config.MaxConnections,
				Port = config.Port
			};
		}

		#region IServerEndpointAdapter Members

		public IBroadcastChannel BroadcastChannel { get; private set; }

		#endregion
	}
}