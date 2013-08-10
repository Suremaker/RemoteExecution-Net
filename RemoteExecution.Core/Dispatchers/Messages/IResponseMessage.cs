namespace RemoteExecution.Core.Dispatchers.Messages
{
	internal interface IResponseMessage : IMessage
	{
		object Value { get; }
	}
}