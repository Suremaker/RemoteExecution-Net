namespace RemoteExecution.Messages
{
	public interface IRequest : IMessage
	{
		object[] Args { get; set; }
		bool IsResponseExpected { get; set; }
		string OperationName { get; set; }
	}
}