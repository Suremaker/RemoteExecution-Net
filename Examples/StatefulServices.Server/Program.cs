using System;
using RemoteExecution.Endpoints;
using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var host = new Host(new ServerEndpointConfig(Protocol.Id, 3132)))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
