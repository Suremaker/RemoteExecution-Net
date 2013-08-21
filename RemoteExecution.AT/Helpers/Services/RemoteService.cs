using System;
using System.Threading;
using RemoteExecution.AT.Helpers.Contracts;

namespace RemoteExecution.AT.Helpers.Services
{
	class RemoteService : IRemoteService
	{
		public void Sleep(TimeSpan time)
		{
			Thread.Sleep(time);
		}
	}
}