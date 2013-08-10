using System;

namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class RequestHandler : IMessageHandler
	{
		public Type InterfaceType { get; private set; }
		public object Handler { get; private set; }

		public RequestHandler(Type interfaceType, object handler)
		{
			InterfaceType = interfaceType;
			Handler = handler;
			HandledMessageType = HandlerGroupId = interfaceType.Name;
		}

		public string HandledMessageType { get; private set; }
		public string HandlerGroupId { get; private set; }
		public void Handle(IMessage message)
		{
			throw new System.NotImplementedException();
		}
	}
}