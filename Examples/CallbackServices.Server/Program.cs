using System;
using CallbackServices.Contracts;
using RemoteExecution.Endpoints;

namespace CallbackServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = new ServerEndpointConfig(Protocol.Id, 3133);
			using (var host = new Host(config))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
