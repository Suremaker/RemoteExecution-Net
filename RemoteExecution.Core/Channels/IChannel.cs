using System;

namespace RemoteExecution.Core.Channels
{
	public interface IChannel : IDisposable
	{
		bool IsOpen { get; }
		event Action ChannelClosed;
		Guid Id { get; }
	}
}