using System;
using System.Net;
using System.Runtime.CompilerServices;
using Lidgren.Network;
using RemoteExecution.Channels;
using RemoteExecution.Serializers;

namespace RemoteExecution.Endpoints.Listeners
{
	public class LidgrenServerConnectionListener : IServerConnectionListener
	{
		private readonly MessageRouter _messageRouter;
		private readonly NetServer _netServer;
		private readonly IMessageSerializer _serializer;
		private MessageLoop _messageLoop;
		public event Action<IDuplexChannel> OnChannelOpen;

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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dispose()
		{
			_netServer.Shutdown("Endpoint disposed");
			_netServer.WaitForClose();

			if (_messageLoop != null)
				_messageLoop.Dispose();
			_messageLoop = null;

		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StartListening()
		{
			if (IsListening)
				return;
			_messageLoop = new MessageLoop(_netServer, _messageRouter.Route);
			_netServer.Start();
		}

		public IBroadcastChannel BroadcastChannel { get; private set; }

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
