using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution.Connections;
using RemoteExecution.Endpoints;
using RemoteExecution.Executors;

namespace OneWayMethodServices.Server
{
	class Host : StatefulServerEndpoint
	{
		public Host(ServerEndpointConfig config)
			: base(config)
		{
		}

		protected override void RegisterServicesFor(INetworkConnection connection)
		{
			var remoteExecutor = connection.RemoteExecutor;
			var twoWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.TwoWay), ConsoleColor.DarkCyan);
			var oneWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.OneWay), ConsoleColor.DarkCyan);
			var longRunningOperation = Aspects.WithTimeMeasure<ILongRunningOperation>(new LongRunningOperation(twoWayCallback, oneWayCallback));

			connection.OperationDispatcher.RegisterRequestHandler(longRunningOperation);
		}
	}
}