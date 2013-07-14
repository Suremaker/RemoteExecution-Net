using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;

namespace OneWayMethodServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			callbackDispatcher.RegisterRequestHandler(Aspects.WithTimeMeasure<IClientCallback>(new ClientCallback()));

			using (var client = new ClientEndpoint(Protocol.Id, callbackDispatcher))
			using (var networkConnection = client.ConnectTo("localhost", 3134))
			{
				var remoteExecutor = new RemoteExecutor(networkConnection);

				var longRunningOperation = Aspects.WithTimeMeasure(remoteExecutor.Create<ILongRunningOperation>(NoResultMethodExecution.OneWay));

				longRunningOperation.Repeat("a", 3);

				longRunningOperation.RepeatWithCallback("b", 3);

				longRunningOperation.RepeatWithOneWayCallback("c", 3);

				Console.WriteLine("Done. Please wait few seconds for callbacks and press enter to exit.");
				Console.ReadLine();
			}
		}
	}
}
