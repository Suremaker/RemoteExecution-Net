using System;
using RemoteExecution.Messaging;

namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : IDisposable
	{
		void StartListening();
		IBroadcastChannel BroadcastChannel { get; }
	}
}