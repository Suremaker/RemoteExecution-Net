using Lidgren.Network;

namespace RemoteExecution.Endpoints
{
	public abstract class ServerEndpoint : LidgrenEndpoint, IServerEndpoint
	{
		public void StartListening()
		{
			Start();
		}

		protected ServerEndpoint(string applicationId, int maxConnections, ushort port)
			: base(new NetServer(new NetPeerConfiguration(applicationId) { MaximumConnections = maxConnections, Port = port }))
		{
		}
	}
}