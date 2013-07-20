using System;
using BroadcastServices.Contracts;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var host = new Host(new ServerEndpointConfig(Protocol.Id, 3135)))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
