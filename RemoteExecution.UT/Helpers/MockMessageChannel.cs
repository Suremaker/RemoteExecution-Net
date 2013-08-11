using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RemoteExecution.Channels;
using RemoteExecution.Messages;

namespace RemoteExecution.UT.Helpers
{
	class MockMessageChannel : IMessageChannel
	{
		public bool IsOpen { get { return true; } }
		public Action<IMessage> OnMessageSend { get; set; }
		public List<IMessage> SentMessages { get; private set; }

		public MockMessageChannel()
		{
			SentMessages = new List<IMessage>();
			OnMessageSend = m => { };
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Send(IMessage message)
		{
			SentMessages.Add(message);
			OnMessageSend(message);
		}

		public event Action<IMessage> Received;
	}
}