using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints
{
	public abstract class StatefulServerEndpoint : ServerEndpoint
	{
		protected StatefulServerEndpoint(ServerEndpointConfig config)
			: base(config)
		{
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		protected sealed override void OnNewConnection(INetworkConnection connection)
		{
			RegisterServicesFor(connection);
		}

		protected abstract void RegisterServicesFor(INetworkConnection connection);
	}
}