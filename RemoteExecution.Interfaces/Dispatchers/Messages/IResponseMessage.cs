namespace RemoteExecution.Dispatchers.Messages
{
	public interface IResponseMessage : IMessage
	{
		object Value { get; }
	}
}