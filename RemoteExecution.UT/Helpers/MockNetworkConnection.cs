using System;
using System.Collections.Generic;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Messaging;

namespace RemoteExecution.UT.Helpers
{
	class MockNetworkConnection : INetworkConnection
	{
		public Action<IMessage> OnMessageSend { get; set; }
		public List<IMessage> SentMessages { get; private set; }

		public MockNetworkConnection(IOperationDispatcher operationDispatcher)
		{
			OperationDispatcher = operationDispatcher;
			SentMessages = new List<IMessage>();
			OnMessageSend = m => { };
		}

		#region IWriteEndpoint Members

		public void Send(IMessage message)
		{
			SentMessages.Add(message);
			OnMessageSend(message);
		}

		#endregion

		public void Dispose()
		{
		}

		public bool IsOpen { get { return true; } }
		public IOperationDispatcher OperationDispatcher { get; private set; }
	}
}