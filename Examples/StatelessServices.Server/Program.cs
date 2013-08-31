using System;
using RemoteExecution;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;
using StatelessServices.Contracts;

namespace StatelessServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Configurator.Configure();

			var dispatcher = new OperationDispatcher()
				.RegisterHandler<ICalculator>(new CalculatorService())
				.RegisterHandler<IGreeter>(new GreeterService());


			using (var host = new StatelessServerEndpoint("net://127.0.0.1:3131/StatelessServices", dispatcher))
			{
				host.Start();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
