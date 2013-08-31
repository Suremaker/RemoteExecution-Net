using System;
using System.Linq;
using BroadcastServices.Contracts;
using Examples.Utils;
using RemoteExecution;
using RemoteExecution.Connections;
using RemoteExecution.Dispatchers;

namespace BroadcastServices.Client
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
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			IBroadcastService broadcastService = Aspects.WithTimeMeasure<IBroadcastService>(new BroadcastService(), ConsoleColor.DarkCyan);
			callbackDispatcher.RegisterHandler(broadcastService);

			using (var client = new ClientConnection("net://localhost:3135/BroadcastServices", callbackDispatcher))
			{
				client.Open();

				var userInfoService = Aspects.WithTimeMeasure(client.RemoteExecutor.Create<IUserInfoService>());
				var registrationService = Aspects.WithTimeMeasure(client.RemoteExecutor.Create<IRegistrationService>());

				GetUsers(userInfoService);
				RegisterUser(registrationService);
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
