using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RemoteExecution.Channels;
using RemoteExecution.Messages;

namespace RemoteExecution.UT.Helpers
{
	class MockMessageChannel : IMessageChannel
	{
		public event Action<IMessage> Received;
		public Action<IMessage> OnMessageSend { get; set; }
		public List<IMessage> SentMessages { get; private set; }

		public MockMessageChannel()
		{
			SentMessages = new List<IMessage>();
			OnMessageSend = m => { };
		}

		#region IMessageChannel Members

		public bool IsOpen { get { return true; } }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Send(IMessage message)
		{
			SentMessages.Add(message);
			OnMessageSend(message);
		}

		#endregion
	}
}