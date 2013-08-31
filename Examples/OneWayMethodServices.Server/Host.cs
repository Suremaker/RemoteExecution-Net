using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution.Core.Config;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Endpoints;
using RemoteExecution.Core.Executors;

namespace OneWayMethodServices.Server
{
	class Host : StatefulServerEndpoint
	{
		public Host(string uri)
			: base(uri, new ServerConfig())
		{
		}
		protected override void InitializeConnection(IRemoteConnection connection)
		{
			var remoteExecutor = connection.RemoteExecutor;
			var twoWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.TwoWay), ConsoleColor.DarkCyan);
			var oneWayCallback = Aspects.WithTimeMeasure(remoteExecutor.Create<IClientCallback>(NoResultMethodExecution.OneWay), ConsoleColor.DarkCyan);
			var longRunningOperation = Aspects.WithTimeMeasure<ILongRunningOperation>(new LongRunningOperation(twoWayCallback, oneWayCallback));

			connection.OperationDispatcher.RegisterHandler(longRunningOperation);
		}
	}
}