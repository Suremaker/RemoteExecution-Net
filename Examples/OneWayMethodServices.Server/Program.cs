using System;

namespace OneWayMethodServices.Server
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var host = new Host(10, 3134))
			{
				host.StartListening();
				Console.WriteLine("Server started...\nPress enter to stop");
				Console.ReadLine();
			}
		}
	}
}
