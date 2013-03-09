using System.Threading.Tasks;
using Lidgren.Network;
using RemoteExecution.Endpoints.Processing;

namespace RemoteExecution.Endpoints
{
	public abstract class LidgrenEndpoint : INetworkEndpoint
	{
		protected readonly NetPeer Peer;
		private MessageLoop _messageLoop;

		protected void Start()
		{
			_messageLoop = new MessageLoop(this);
			Peer.Start();
		}

		protected LidgrenEndpoint(NetPeer peer)
		{
			Peer = peer;
			_messageLoop = new MessageLoop(this);
		}

		public void Dispose()
		{
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
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