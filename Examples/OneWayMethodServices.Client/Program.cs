using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using RemoteExecution.Executors;

namespace OneWayMethodServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			callbackDispatcher.RegisterRequestHandler(Aspects.WithTimeMeasure<IClientCallback>(new ClientCallback(), ConsoleColor.DarkCyan));

			using (var client = new ClientEndpoint(Protocol.Id, callbackDispatcher))
			{
				client.ConnectTo("localhost", 3134);

				var longRunningOperation = Aspects.WithTimeMeasure(client.RemoteExecutor.Create<ILongRunningOperation>(NoResultMethodExecution.OneWay));

				longRunningOperation.Repeat("a", 3);
				longRunningOperation.RepeatWithCallback("b", 3);
				longRunningOperation.RepeatWithOneWayCallback("c", 3);

				Console.WriteLine("Done. Please wait few seconds for callbacks and press enter to exit.");
				Console.ReadLine();
			}
		}
	}
}
