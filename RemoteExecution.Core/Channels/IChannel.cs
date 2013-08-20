using System;

namespace RemoteExecution.Core.Channels
{
	public interface IChannel : IDisposable
	{
		event Action ChannelClosed;
		Guid Id { get; }
		bool IsOpen { get; }
	}
}