using System;
using RemoteExecution.Channels;

namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		void StartListening();
		IBroadcastChannel BroadcastChannel { get; }
	}
}