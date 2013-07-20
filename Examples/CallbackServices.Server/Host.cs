using CallbackServices.Contracts;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;

namespace CallbackServices.Server
{
	class Host : StatefulServerEndpoint
	{
		public Host(ServerEndpointConfig config)
			: base(config)
		{
		}

		protected override void RegisterServicesFor(INetworkConnection connection)
		{
			var clientCallback = connection.RemoteExecutor.Create<IClientCallback>();
			connection.OperationDispatcher.RegisterRequestHandler<ILongRunningOperation>(new LongRunningOperation(clientCallback));
		}
	}
}