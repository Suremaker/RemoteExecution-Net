namespace RemoteExecution.Messaging
{
	public interface IResponse : IMessage
	{
		object Value { get; }
	}
}