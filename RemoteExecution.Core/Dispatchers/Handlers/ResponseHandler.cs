using System;
using System.Threading;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class ResponseHandler : IResponseHandler
	{
		private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
		private IResponseMessage _response;

		public ResponseHandler(Guid handlerGroupId)
		{
			HandlerGroupId = handlerGroupId.ToString();
			HandledMessageType = Guid.NewGuid().ToString();
		}

		public string HandledMessageType { get; private set; }
		public string HandlerGroupId { get; private set; }

		public void Handle(IMessage msg)
		{
			_response = ((IResponseMessage)msg);
			_resetEvent.Set();
		}

		public object GetValue()
		{
			return _response.Value;
		}

		public void WaitForResponse()
		{
			_resetEvent.Wait();
		}
	}
}