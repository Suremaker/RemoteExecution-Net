using System;

namespace RemoteExecution.Channels
{
	public interface IChannel : IDisposable
	{
		event Action ChannelClosed;
		Guid Id { get; }
		bool IsOpen { get; }
	}
}