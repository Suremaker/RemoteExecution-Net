using Lidgren.Network;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Lidgren.Channels
{
	public class LidgrenClientChannel : LidgrenDuplexChannel, IClientChannel
	{
		private readonly string _host;
		private readonly ushort _port;
		private readonly NetClient _client;
		private MessageLoop _messageLoop;

		public LidgrenClientChannel(string applicationId, string host, ushort port, IMessageSerializer serializer)
			: base(serializer)
		{
			_host = host;
			_port = port;
			_client = new NetClient(new NetPeerConfiguration(applicationId));
		}

		public void Open()
		{
			_messageLoop = new MessageLoop(_client, HandleMessage);
			_client.Start();
			Connection = _client.Connect(_host, _port);
		}

		private void HandleMessage(NetIncomingMessage msg)
		{
			if (msg.MessageType == NetIncomingMessageType.Data)
				HandleIncomingMessage(msg);
		}

		protected override void Close()
		{
			base.Close();
			_client.Disconnect("Client disposed");
			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}
	}
}