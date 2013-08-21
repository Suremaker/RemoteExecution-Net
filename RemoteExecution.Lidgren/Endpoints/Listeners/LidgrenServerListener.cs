using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Lidgren.Network;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Endpoints.Listeners;
using RemoteExecution.Core.Serializers;
using RemoteExecution.Lidgren.Channels;

namespace RemoteExecution.Lidgren.Endpoints.Listeners
{
	public class LidgrenServerListener : IServerListener
	{
		private static readonly TimeSpan _synchronizationTimeSpan = TimeSpan.FromMilliseconds(25);

		private readonly NetServer _netServer;
		private readonly IMessageSerializer _serializer;
		private MessageLoop _messageLoop;
		public event Action<IDuplexChannel> OnChannelOpen;

		public LidgrenServerListener(string applicationId, ushort port, IMessageSerializer serializer)
		{
			var netConfig = new NetPeerConfiguration(applicationId)
			{
				MaximumConnections = int.MaxValue,
				Port = port
			};
			_serializer = serializer;
			_netServer = new NetServer(netConfig);
		}

		#region IServerListener Members

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
			_messageLoop = new MessageLoop(_netServer, HandleMessage);
			_netServer.Start();
		}

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
			channel.Dispose();
		}

		private void HandleData(NetIncomingMessage message)
		{
			ExtractChannelWithWait(message.SenderConnection).HandleIncomingMessage(message);
		}

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
