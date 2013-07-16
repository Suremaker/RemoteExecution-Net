using System;
using System.Linq;
using BroadcastServices.Contracts;
using Examples.Utils;
using RemoteExecution.Dispatchers;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			IOperationDispatcher callbackDispatcher = new OperationDispatcher();
			IBroadcastService broadcastService = Aspects.WithTimeMeasure<IBroadcastService>(new BroadcastService(), ConsoleColor.DarkCyan);
			callbackDispatcher.RegisterRequestHandler(broadcastService);

			using (var client = new ClientEndpoint(Protocol.Id, callbackDispatcher))
			{
				client.ConnectTo("localhost", 3135);

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
	}
}
