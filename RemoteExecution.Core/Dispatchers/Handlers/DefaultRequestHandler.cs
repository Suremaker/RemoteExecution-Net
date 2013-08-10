namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class DefaultRequestHandler : IMessageHandler
	{
		public string HandledMessageType { get; private set; }
		public string HandlerGroupId { get; private set; }
		public void Handle(IMessage message)
		{
			throw new System.NotImplementedException();
		}
	}
}