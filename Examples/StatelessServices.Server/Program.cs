using System;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using StatelessServices.Contracts;

namespace StatelessServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var dispatcher = new OperationDispatcher()
				.RegisterRequestHandler<ICalculator>(new CalculatorService())
				.RegisterRequestHandler<IGreeter>(new GreeterService());

			var config = new ServerEndpointConfig(Protocol.Id, 3131);

			using (var host = new StatelessServerEndpoint(config, dispatcher))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
