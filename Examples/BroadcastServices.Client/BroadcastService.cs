using System;
using BroadcastServices.Contracts;

namespace BroadcastServices.Client
{
	internal class BroadcastService : IBroadcastService
	{
		public void UserRegistered(string name)
		{
			Console.WriteLine("New user registered: {0}", name);
		}

		public void UserLeft(string name)
		{
			Console.WriteLine("Registered user left: {0}", name);
		}
	}
}