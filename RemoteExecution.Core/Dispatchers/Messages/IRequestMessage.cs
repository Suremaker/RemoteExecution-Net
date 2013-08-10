using RemoteExecution.Core.Channels;

namespace RemoteExecution.Core.Dispatchers.Messages
{
	internal interface IRequestMessage : IMessage
	{
		object[] Args { get; }
		string MethodName { get; }
		bool IsResponseExpected { get; }
		IChannelProvider ChannelProvider { get; set; }
	}
}