using System;
using System.Linq;
using RemoteExecution;
using RemoteExecution.Core.Connections;
using StatefulServices.Contracts;

namespace StatefulServices.Client
{
	class Program
	{
		private static void GetUsers(IUserInfoService userInfoService)
		{
			try
			{
				Console.WriteLine("Registered users:");
				Console.WriteLine(string.Join("\n", userInfoService.GetRegisteredUsers().OrderBy(u => u)));
			}
			catch (Exception e)
			{
				Console.WriteLine("Unable to get users: {0}", e.Message);
			}
		}

		static void Main(string[] args)
		{
			Configurator.Configure();
			using (var client = new ClientConnection("net://localhost:3132/StatefulServices"))
			{
				client.Open();

				var userInfoService = client.RemoteExecutor.Create<IUserInfoService>();
				GetUsers(userInfoService);
				RegisterUser(client.RemoteExecutor.Create<IRegistrationService>());
				GetUsers(userInfoService);

				Console.WriteLine("Done. Press enter to exit.");
				Console.ReadLine();
			}
		}

		private static void RegisterUser(IRegistrationService registrationService)
		{
			Console.WriteLine("Type user name:");
			registrationService.Register(Console.ReadLine());
			Console.WriteLine("Registered as: {0}", registrationService.GetUserName());
		}
	}
}
