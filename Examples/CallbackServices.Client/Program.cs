using System;
using CallbackServices.Contracts;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;

namespace CallbackServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			callbackDispatcher.RegisterRequestHandler<IClientCallback>(new ClientCallback());

			using (var client = new ClientEndpoint(Protocol.Id, callbackDispatcher))
			{
				client.ConnectTo("localhost", 3133);

				var userInfoService = client.RemoteExecutor.Create<ILongRunningOperation>();
				userInfoService.Perform(5);
				Console.WriteLine("Done. Press enter to exit.");
				Console.ReadLine();
			}
		}
	}
}
