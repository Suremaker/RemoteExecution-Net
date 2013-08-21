using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Lidgren.Channels
{
	public class LidgrenClientChannel : LidgrenDuplexChannel, IClientChannel
	{
		private readonly NetClient _client;
		private readonly string _host;
		private readonly ushort _port;
		private readonly MessageRouter _messageRouter;
		private MessageLoop _messageLoop;

		public LidgrenClientChannel(string applicationId, string host, ushort port, IMessageSerializer serializer)
			: base(serializer)
		{
			_host = host;
			_port = port;
			_client = new NetClient(new NetPeerConfiguration(applicationId));
			_messageRouter = new MessageRouter();
			_messageRouter.DataReceived += HandleIncomingMessage;
			_messageRouter.ConnectionClosed += c => OnConnectionClose();
		}

		#region IClientChannel Members

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Open()
		{
			if (IsOpen)
				throw new InvalidOperationException("Channel already opened.");
			_messageLoop = new MessageLoop(_client, _messageRouter.Route);
			_client.Start();
			Connection = _client.Connect(_host, _port);
			Connection.WaitForConnectionToOpen();
		}

		#endregion

		protected override void Close()
		{
			base.Close();
			_client.Shutdown("Client disposed");
			_client.WaitForClose();

			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;
		}
	}
}