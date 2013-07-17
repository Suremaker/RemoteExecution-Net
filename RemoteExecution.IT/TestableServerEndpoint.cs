using System.Linq;
using System.Runtime.CompilerServices;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	class TestableServerEndpoint : ServerEndpoint
	{
		public TestableServerEndpoint(string appId, int maxConnections, ushort port)
			: base(appId, maxConnections, port)
		{
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void OnNewConnection(INetworkConnection connection)
		{
			var remoteService = new RemoteService(
				ActiveConnections.Count(),
				connection.RemoteExecutor.Create<IClientService>(),
				BroadcastRemoteExecutor.Create<IBroadcastService>(),
				connection);
			connection.OperationDispatcher.RegisterRequestHandler(LoggingProxy.For<IRemoteService>(remoteService, "SERVER"));
			connection.OperationDispatcher.RegisterRequestHandler<ICalculatorService>(new CalculatorService());
		}
	}
}