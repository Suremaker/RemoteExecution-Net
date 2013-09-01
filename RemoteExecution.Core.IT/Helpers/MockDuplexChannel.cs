using System;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Core.IT.Helpers
{
	class MockDuplexChannel : IDuplexChannel
	{
		public event Action Closed;
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