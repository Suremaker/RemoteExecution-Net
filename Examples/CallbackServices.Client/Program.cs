using System;
using CallbackServices.Contracts;
using RemoteExecution;
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
			using (var networkConnection = client.ConnectTo("localhost", 3133))
			{
				var remoteExecutor = new RemoteExecutor(networkConnection);

				var userInfoService = remoteExecutor.Create<ILongRunningOperation>();
				userInfoService.Perform(5);
				Console.WriteLine("Done. Press enter to exit.");
				Console.ReadLine();
			}
		}
	}
}
