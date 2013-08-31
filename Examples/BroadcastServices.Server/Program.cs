using System;
using RemoteExecution;

namespace BroadcastServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Configurator.Configure();
			using (var host = new Host("net://127.0.0.1:3135/BroadcastServices"))
			{
				host.Start();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
