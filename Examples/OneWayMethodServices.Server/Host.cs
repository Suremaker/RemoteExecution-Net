using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Executors;

namespace OneWayMethodServices.Server
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
			var remoteExecutor = connection.RemoteExecutor;
			var twoWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.TwoWay), ConsoleColor.DarkCyan);
			var oneWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.OneWay), ConsoleColor.DarkCyan);
			var longRunningOperation = Aspects.WithTimeMeasure<ILongRunningOperation>(new LongRunningOperation(twoWayCallback, oneWayCallback));

			connection.OperationDispatcher.RegisterRequestHandler(longRunningOperation);
		}
	}
}