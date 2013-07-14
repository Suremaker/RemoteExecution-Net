using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution;
using RemoteExecution.Endpoints;

namespace OneWayMethodServices.Server
{
	class Host : ServerEndpoint
	{
		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			var remoteExecutor = new RemoteExecutor(connection);
			var twoWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.TwoWay), ConsoleColor.DarkCyan);
			var oneWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.OneWay), ConsoleColor.DarkCyan);
			var longRunningOperation = Aspects.WithTimeMeasure<ILongRunningOperation>(new LongRunningOperation(twoWayCallback, oneWayCallback));

			connection.OperationDispatcher.RegisterRequestHandler(longRunningOperation);
			return true;
		}
	}
}