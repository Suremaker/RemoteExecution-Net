using CallbackServices.Contracts;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;

namespace CallbackServices.Server
{
	class Host : ServerEndpoint
	{
		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		protected override void OnNewConnection(INetworkConnection connection)
		{
			var clientCallback = connection.RemoteExecutor.Create<IClientCallback>();
			connection.OperationDispatcher.RegisterRequestHandler<ILongRunningOperation>(new LongRunningOperation(clientCallback));
		}
	}
}