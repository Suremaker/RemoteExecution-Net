using RemoteExecution.Core.Channels;

namespace RemoteExecution.Core.Dispatchers.Messages
{
	internal interface IRequestMessage : IMessage
	{
		object[] Args { get; }
		IOutputChannel Channel { get; set; }
		bool IsResponseExpected { get; }
		string MethodName { get; }
	}
}