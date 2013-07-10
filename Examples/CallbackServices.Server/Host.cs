using CallbackServices.Contracts;
using RemoteExecution;
using RemoteExecution.Endpoints;

namespace CallbackServices.Server
{
	class Host : ServerEndpoint
	{
		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			var clientCallback = new RemoteExecutor(connection).Create<IClientCallback>();
			connection.OperationDispatcher.RegisterRequestHandler<ILongRunningOperation>(new LongRunningOperation(clientCallback));
			return true;
		}
	}
}