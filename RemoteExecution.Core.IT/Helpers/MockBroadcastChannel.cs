using System;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Core.IT.Helpers
{
	class MockBroadcastChannel : IBroadcastChannel
	{
		public event Action ChannelClosed;
		public event Action<IMessage> OnSend;

		public MockBroadcastChannel()
		{
			Id = Guid.NewGuid();
		}

		#region IBroadcastChannel Members

		public void Dispose()
		{
		}

		public Guid Id { get; private set; }
		public bool IsOpen { get { return true; } }
		public void Send(IMessage message)
		{
			OnSend(message);
		}

		public int ReceiverCount { get { return 1; } }

		#endregion
	}
}