using System;

namespace RemoteExecution.AT.Helpers
{
	public static class SyncHelper
	{
		public static void WaitUntil(Func<bool> func, int timeoutInMs = 500)
		{
			var time = DateTime.UtcNow;
			var timeout = TimeSpan.FromMilliseconds(timeoutInMs);
			while (DateTime.UtcNow - time < timeout)
			{
				if (func())
					return;
			}
			throw new TimeoutException();
		}
	}
}