using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Core.Channels
{
	public abstract class OutputChannel : IOutputChannel
	{
		protected readonly IMessageSerializer Serializer;

		protected OutputChannel(IMessageSerializer serializer)
		{
			Serializer = serializer;
			Id = Guid.NewGuid();
		}

		public void Dispose()
		{
			Close();
		}

		protected abstract void Close();
		public abstract bool IsOpen { get; }
		public event Action ChannelClosed;
		public Guid Id { get; private set; }

		public void Send(IMessage message)
		{
			if (!IsOpen)
				throw new NotConnectedException("Channel is closed.");

			SendData(Serializer.Serialize(message));
		}

		protected abstract void SendData(byte[] data);

		protected void FireChannelClosed()
		{
			if (ChannelClosed != null)
				ChannelClosed();
		}
	}
}