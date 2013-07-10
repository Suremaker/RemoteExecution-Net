using System;
using System.Threading;
using CallbackServices.Contracts;

namespace CallbackServices.Server
{
	internal class LongRunningOperation : ILongRunningOperation
	{
		private readonly IClientCallback _clientCallback;

		public LongRunningOperation(IClientCallback clientCallback)
		{
			_clientCallback = clientCallback;
		}

		public void Perform(int steps)
		{
			for (int i = 1; i <= steps; ++i)
			{
				_clientCallback.Progress(i);
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}
	}
}