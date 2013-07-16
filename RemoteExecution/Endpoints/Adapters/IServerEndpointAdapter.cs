using RemoteExecution.Channels;

namespace RemoteExecution.Endpoints.Adapters
{
	public interface IServerEndpointAdapter : IEndpointAdapter
	{
		IBroadcastChannel BroadcastChannel { get; }
	}
}