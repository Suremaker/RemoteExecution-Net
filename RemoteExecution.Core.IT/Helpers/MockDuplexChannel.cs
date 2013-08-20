using System;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.IT.Helpers
{
	class MockDuplexChannel : IDuplexChannel
	{
		public event Action ChannelClosed;
		public event Action<IMessage> OnSend;
		public event Action<IMessage> Received;

		public MockDuplexChannel()
		{
			Id = Guid.NewGuid();
		}

		#region IDuplexChannel Members

		public void Dispose()
		{
		}

		public bool IsOpen { get { return true; } }
		public Guid Id { get; private set; }
		public void Send(IMessage message)
		{
			OnSend(message);
		}

		#endregion
	}
}