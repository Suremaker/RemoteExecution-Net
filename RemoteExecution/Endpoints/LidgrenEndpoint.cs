using System.Threading.Tasks;
using Lidgren.Network;

namespace RemoteExecution.Endpoints
{
	public abstract class LidgrenEndpoint : INetworkEndpoint
	{
		protected readonly NetPeer Peer;

		protected void Start()
		{
			Peer.Start();
		}

		protected LidgrenEndpoint(NetPeer peer)
		{
			Peer = peer;
		}

		public void Dispose()
		{
			Peer.Shutdown("Endpoint disposed");
		}

		public bool ProcessMessage()
		{
			var msg = Peer.ReadMessage();
			if (msg == null)
				return false;
			Task.Factory.StartNew(() => HandleMessage(msg));
			return true;
		}

		protected abstract void HandleData(NetIncomingMessage message);
		protected virtual void HandleClosedConnection(NetConnection connection) { }
		protected virtual void HandleNewConnection(NetConnection connection) { }

		private void HandleMessage(NetIncomingMessage msg)
		{
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.Data:
					HandleData(msg);
					break;
				case NetIncomingMessageType.StatusChanged:
					HandleStatusChange(msg);
					break;
			}
		}

		private void HandleStatusChange(NetIncomingMessage msg)
		{
			switch ((NetConnectionStatus)msg.ReadByte())
			{
				case NetConnectionStatus.Connected:
					HandleNewConnection(msg.SenderConnection);
					break;
				case NetConnectionStatus.Disconnected:
					HandleClosedConnection(msg.SenderConnection);
					break;
			}

		}
	}
}