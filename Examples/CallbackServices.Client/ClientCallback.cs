using System;
using CallbackServices.Contracts;

namespace CallbackServices.Client
{
	internal class ClientCallback : IClientCallback
	{
		public void Progress(int step)
		{
			Console.WriteLine("Performing step: {0}...", step);
		}
	}
}