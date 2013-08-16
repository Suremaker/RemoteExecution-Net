using System;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class DefaultRequestHandler : IMessageHandler
	{
		public string HandledMessageType { get { return string.Empty; } }
		public Guid HandlerGroupId { get { return Guid.Empty; } }

		public void Handle(IMessage message)
		{
			var request = message as IRequestMessage;
			if (request == null || !request.IsResponseExpected)
				return;

			string errorMessage = string.Format("No handler is defined for {0} type.", request.MessageType);
			request.Channel.Send(new ExceptionResponseMessage(request.CorrelationId, typeof(InvalidOperationException), errorMessage));
		}
	}
}