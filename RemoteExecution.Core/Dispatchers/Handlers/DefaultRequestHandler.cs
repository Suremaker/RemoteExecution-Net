using System;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Dispatchers.Handlers
{
	internal class DefaultRequestHandler : IMessageHandler
	{
		#region IMessageHandler Members

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

		#endregion
	}
}