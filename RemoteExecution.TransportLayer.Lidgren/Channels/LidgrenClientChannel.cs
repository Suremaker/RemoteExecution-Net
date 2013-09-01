using System;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Lidgren client channel allowing to send and receive messages. 
	/// Client channel has to be opened with Open() method before use
	/// and can be reopened after it has been closed.
	/// </summary>
	public class LidgrenClientChannel : LidgrenDuplexChannel, IClientChannel
	{
		private readonly NetClient _client;
		private readonly string _host;
		private readonly MessageRouter _messageRouter;
		private readonly ushort _port;
		private MessageLoop _messageLoop;

		/// <summary>
		/// Creates client channel instance.
		/// </summary>
		/// <param name="applicationId">Application id that has to match to one used by <see cref="LidgrenServerConnectionListener"/>.</param>
		/// <param name="host">Host to connect to.</param>
		/// <param name="port">Port to connect to.</param>
		/// <param name="serializer">Message serializer.</param>
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

		/// <summary>
		/// Opens channel for sending and receiving messages.
		/// If channel has been closed, this method reopens it.
		/// </summary>
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

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
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