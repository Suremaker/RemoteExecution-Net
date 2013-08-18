using System.IO;

namespace RemoteExecution.Core.Channels
{
	public class NotConnectedException : IOException
	{
		public NotConnectedException(string message)
			: base(message)
		{
		}
	}
}