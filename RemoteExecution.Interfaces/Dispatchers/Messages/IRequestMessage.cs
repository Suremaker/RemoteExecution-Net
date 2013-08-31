using RemoteExecution.Channels;

namespace RemoteExecution.Dispatchers.Messages
{
	public interface IRequestMessage : IMessage
	{
		object[] Args { get; }
		IOutputChannel Channel { get; set; }
		bool IsResponseExpected { get; }
		string MethodName { get; }
	}
}