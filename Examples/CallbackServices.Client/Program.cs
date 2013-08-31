using System;
using CallbackServices.Contracts;
using RemoteExecution;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;

namespace CallbackServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			Configurator.Configure();

			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			callbackDispatcher.RegisterHandler<IClientCallback>(new ClientCallback());

			using (var client = new ClientConnection("net://localhost:3133/CallbackServices", callbackDispatcher))
			{
				client.Open();

				var userInfoService = client.RemoteExecutor.Create<ILongRunningOperation>();
				userInfoService.Perform(5);
				Console.WriteLine("Done. Press enter to exit.");
				Console.ReadLine();
			}
		}
	}
}
