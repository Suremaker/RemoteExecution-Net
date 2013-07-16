namespace RemoteExecution.Messages
{
	public interface IRequest : IMessage
	{
		object[] Args { get; set; }
		string OperationName { get; set; }
		bool IsResponseExpected { get; set; }
	}
}