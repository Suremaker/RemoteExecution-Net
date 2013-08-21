using System;
using Lidgren.Network;

namespace RemoteExecution.Lidgren
{
	internal class MessageRouter
	{
		public event Action<NetIncomingMessage> DataReceived;
		public event Action<NetConnection> ConnectionClosed;
		public event Action<NetConnection> ConnectionOpened;

		public void Route(NetIncomingMessage msg)
		{
			switch (msg.MessageType)
			{
				case NetIncomingMessageType.Data:
					HandleIncomingData(msg);
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

		private void HandleClosedConnection(NetConnection connection)
		{
			if (ConnectionClosed != null)
				ConnectionClosed(connection);
		}

		private void HandleNewConnection(NetConnection connection)
		{
			if (ConnectionOpened != null)
				ConnectionOpened(connection);
		}

		private void HandleIncomingData(NetIncomingMessage msg)
		{
			if (DataReceived != null)
				DataReceived(msg);
		}
	}
}