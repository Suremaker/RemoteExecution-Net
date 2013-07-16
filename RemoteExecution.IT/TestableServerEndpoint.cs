using System.Collections.Generic;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	class TestableServerEndpoint : ServerEndpoint
	{
		public List<INetworkConnection> ActiveConnections { get; private set; }
		public TestableServerEndpoint(string appId, int maxConnections, ushort port)
			: base(appId, maxConnections, port)
		{
			ActiveConnections = new List<INetworkConnection>();
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return new OperationDispatcher();
		}

		protected override bool OnNewConnection(INetworkConnection connection)
		{
			ActiveConnections.Add(connection);

			var remoteService = new RemoteService(
				ActiveConnections.Count,
				connection.RemoteExecutor.Create<IClientService>(),
				new BroadcastRemoteExecutor(BroadcastChannel).Create<IBroadcastService>(),
				connection);
			connection.OperationDispatcher.RegisterRequestHandler(LoggingProxy.For<IRemoteService>(remoteService, "SERVER"));
			connection.OperationDispatcher.RegisterRequestHandler<ICalculatorService>(new CalculatorService());
			return true;
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			ActiveConnections.Remove(connection);
		}
	}
}