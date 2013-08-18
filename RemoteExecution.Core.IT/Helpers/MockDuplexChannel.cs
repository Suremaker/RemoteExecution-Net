using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.IT.Helpers
{
	class MockDuplexChannel : IDuplexChannel
	{
		public MockDuplexChannel()
		{
			Id = Guid.NewGuid();
		}

		public void Dispose()
		{
		}

		public bool IsOpen { get { return true; } }
		public event Action ChannelClosed;
		public event Action<IMessage> OnSend;
		public Guid Id { get; private set; }
		public void Send(IMessage message)
		{
			OnSend(message);
		}

		public event Action<IMessage> Received;
	}
}