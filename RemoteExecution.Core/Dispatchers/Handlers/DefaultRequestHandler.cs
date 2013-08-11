using System;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class DefaultRequestHandler : IMessageHandler
	{
		public string HandledMessageType { get { return string.Empty; } }
		public string HandlerGroupId { get { return string.Empty; } }

		public void Handle(IMessage message)
		{
			var request = message as IRequestMessage;
			if (request == null || !request.IsResponseExpected)
				return;

			string errorMessage = string.Format("No handler is defined for {0} type.", request.MessageType);
			request.ChannelProvider.GetOutgoingChannel().Send(new ExceptionResponseMessage(request.CorrelationId, typeof(InvalidOperationException), errorMessage));
		}
	}
}