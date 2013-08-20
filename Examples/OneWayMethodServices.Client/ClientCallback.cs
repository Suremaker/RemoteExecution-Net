using System;
using System.Threading;
using OneWayMethodServices.Contracts;

namespace OneWayMethodServices.Client
{
	internal class ClientCallback : IClientCallback
	{
		#region IClientCallback Members

		public void FinishRepetition(string result)
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
			Console.WriteLine("Result: " + result);
		}

		#endregion
	}
}