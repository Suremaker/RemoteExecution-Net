namespace RemoteExecution.Messaging
{
	public interface IMessageChannel
	{
		void Send(IMessage message);
	}
}