using System;

namespace RemoteExecution.Core.Channels
{
	public interface IChannelProvider
	{
		IOutgoingMessageChannel GetOutgoingChannel();
		IIncomingMessageChannel GetIncomingChannel();
		Guid Id { get; }
	}
}
