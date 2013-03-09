using System;
using System.Threading;
using RemoteExecution.Messaging;

namespace RemoteExecution.Handling
{
	internal class ResponseHandler : IHandler
	{
		private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
		private IResponse _response;

		public ResponseHandler()
		{
			Id = Guid.NewGuid().ToString();
		}

		#region IHandler Members

		public string Id { get; private set; }

		public void Handle(IMessage msg, IMessageSender messageSender)
		{
			_response = ((IResponse)msg);
			_resetEvent.Set();
		}

		#endregion

		public object GetValue()
		{
			return _response.Value;
		}

		public void Wait()
		{
			_resetEvent.Wait();
		}
	}
}