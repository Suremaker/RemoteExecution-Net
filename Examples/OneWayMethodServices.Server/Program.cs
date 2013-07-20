using System;
using OneWayMethodServices.Contracts;
using RemoteExecution.Endpoints;

namespace OneWayMethodServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var host = new Host(new ServerEndpointConfig(Protocol.Id, 3134)))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
