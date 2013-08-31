using System;
using Examples.Utils;
using OneWayMethodServices.Contracts;
using RemoteExecution;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Executors;

namespace OneWayMethodServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			Configurator.Configure();
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			callbackDispatcher.RegisterHandler(Aspects.WithTimeMeasure<IClientCallback>(new ClientCallback(), ConsoleColor.DarkCyan));

			using (var client = new ClientConnection("net://localhost:3134/CallbackDispatcher", callbackDispatcher))
			{
				client.Open();

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
