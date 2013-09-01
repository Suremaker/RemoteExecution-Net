using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	public abstract class OutputChannel : IOutputChannel
	{
		protected readonly IMessageSerializer Serializer;
		public event Action Closed;

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
			if (Closed != null)
				Closed();
		}

		protected abstract void SendData(byte[] data);
	}
}