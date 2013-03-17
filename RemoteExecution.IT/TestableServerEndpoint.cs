using System.Collections.Generic;
using RemoteExecution.Endpoints;
using RemoteExecution.IT.Services;

namespace RemoteExecution.IT
{
	class TestableServerEndpoint:ServerEndpoint
	{
		public List<INetworkConnection> ActiveConnections { get; private set; }
		public TestableServerEndpoint(string appId, int maxConnections, ushort port) : base(appId, maxConnections, port)
		{
			ActiveConnections=new List<INetworkConnection>();
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			ActiveConnections.Add(connection);
			
			var remoteService = new RemoteService(ActiveConnections.Count,new RemoteExecutor(connection).Create<IClientService>(),connection);
			connection.OperationDispatcher.RegisterRequestHandler(LoggingProxy.For<IRemoteService>(remoteService));
			connection.OperationDispatcher.RegisterRequestHandler<ICalculatorService>(new CalculatorService());
			return true;
		}

		protected override void OnConnectionClose(INetworkConnection connection)
		{
			ActiveConnections.Remove(connection);
		}
	}
}