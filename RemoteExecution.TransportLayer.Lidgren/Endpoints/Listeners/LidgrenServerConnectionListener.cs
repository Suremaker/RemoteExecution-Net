using System;
using System.Net;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Serializers;

namespace RemoteExecution.Endpoints.Listeners
{
	/// <summary>
	/// Lidgren server connection listener class allowing to listen for new connection, handle them and use broadcast channel.
	/// </summary>
	public class LidgrenServerConnectionListener : IServerConnectionListener
	{
		private readonly MessageRouter _messageRouter;
		private readonly NetServer _netServer;
		private readonly IMessageSerializer _serializer;
		private MessageLoop _messageLoop;

		/// <summary>
		/// Fires when channel for new connection is opened.
		/// After all event handlers are finished, channel is treated as fully operational and ready for receiving data.
		/// </summary>
		public event Action<IDuplexChannel> OnChannelOpen;

		/// <summary>
		/// Creates listener instance.
		/// </summary>
		/// <param name="applicationId">Application id that would be used to accept/reject incoming connections.</param>
		/// <param name="listenAddress">IP address on which listener would be listening for incoming connections. Use 0.0.0.0 to listen on all network interfaces.</param>
		/// <param name="port">Port on which listener would be listening for incoming connections.</param>
		/// <param name="serializer">Message serializer.</param>
		public LidgrenServerConnectionListener(string applicationId, string listenAddress, ushort port, IMessageSerializer serializer)
		{
			var netConfig = new NetPeerConfiguration(applicationId)
			{
				MaximumConnections = int.MaxValue,
				Port = port,
				LocalAddress = IPAddress.Parse(listenAddress)
			};
			_serializer = serializer;
			_netServer = new NetServer(netConfig);

			_messageRouter = new MessageRouter();
			_messageRouter.ConnectionClosed += HandleClosedConnection;
			_messageRouter.ConnectionOpened += HandleNewConnection;
			_messageRouter.DataReceived += HandleReceivedData;

			BroadcastChannel = new LidgrenBroadcastChannel(_netServer, serializer);
		}

		#region IServerConnectionListener Members

		/// <summary>
		/// Closes connection listener and shuts down lidgren net server and its message loop.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dispose()
		{
			_netServer.Shutdown("Endpoint disposed");
			_netServer.WaitForClose();

			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;

		}

		/// <summary>
		/// Starts listening for incoming connections.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StartListening()
		{
			if (IsListening)
				return;
			_messageLoop = new MessageLoop(_netServer, _messageRouter.Route);
			_netServer.Start();
		}

		/// <summary>
		/// Returns broadcast channel allowing to send messages to all clients at once.
		/// </summary>
		public IBroadcastChannel BroadcastChannel { get; private set; }

		/// <summary>
		/// Returns true if listener is actively listening for incoming connections.
		/// </summary>
		public bool IsListening { get { return _netServer.Status == NetPeerStatus.Running; } }

		#endregion

		private LidgrenDuplexChannel ExtractChannel(NetConnection netConnection)
		{
			return netConnection.Tag as LidgrenDuplexChannel;
		}

		private LidgrenDuplexChannel ExtractChannelWithWait(NetConnection netConnection)
		{
			var channel = ExtractChannel(netConnection);
			if (channel != null)
				return channel;

			lock (netConnection)
				return ExtractChannel(netConnection);
		}

		private void HandleClosedConnection(NetConnection netConnection)
		{
			var channel = ExtractChannelWithWait(netConnection);
			netConnection.Tag = null;
			channel.OnConnectionClose();
		}

		private void HandleNewConnection(NetConnection netConnection)
		{
			lock (netConnection)
			{
				var channel = new LidgrenDuplexChannel(netConnection, _serializer);
				if (OnChannelOpen != null)
					OnChannelOpen(channel);
				netConnection.Tag = channel;
			}
		}

		private void HandleReceivedData(NetIncomingMessage message)
		{
			ExtractChannelWithWait(message.SenderConnection).HandleIncomingMessage(message);
		}
	}
}
