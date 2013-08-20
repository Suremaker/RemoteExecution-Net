using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Core.Channels
{
	public abstract class OutputChannel : IOutputChannel
	{
		protected readonly IMessageSerializer Serializer;
		public event Action ChannelClosed;

		protected OutputChannel(IMessageSerializer serializer)
		{
			Serializer = serializer;
			Id = Guid.NewGuid();
		}

		#region IOutputChannel Members

		public void Dispose()
		{
			Close();
		}

		public abstract bool IsOpen { get; }
		public Guid Id { get; private set; }

		public void Send(IMessage message)
		{
			if (!IsOpen)
				throw new NotConnectedException("Channel is closed.");

			SendData(Serializer.Serialize(message));
		}

		#endregion

		protected abstract void Close();

		protected void FireChannelClosed()
		{
			if (ChannelClosed != null)
				ChannelClosed();
		}

		protected abstract void SendData(byte[] data);
	}
}