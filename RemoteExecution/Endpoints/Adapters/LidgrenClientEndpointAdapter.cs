using System.IO;
using System.Linq;
using System.Threading;
using Lidgren.Network;

namespace RemoteExecution.Endpoints.Adapters
{
	internal class LidgrenClientEndpointAdapter : LidgrenEndpointAdapter, IClientEndpointAdapter
	{
		public LidgrenClientEndpointAdapter(string applicationId)
			: base(new NetClient(new NetPeerConfiguration(applicationId)))
		{
		}

		public void ConnectTo(string host, ushort port)
		{
			NetConnection conn = Peer.Connect(host, port);
			while (conn.Status != NetConnectionStatus.Connected || !ActiveConnections.Any())
			{
				if (conn.Status == NetConnectionStatus.Disconnected)
					throw new IOException("Connection closed.");

				Thread.Sleep(150);
			}
		}
	}
}