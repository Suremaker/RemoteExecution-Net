namespace RemoteExecution.Messages
{
	public interface IResponse : IMessage
	{
		object Value { get; }
	}
}