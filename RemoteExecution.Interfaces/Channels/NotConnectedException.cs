using System.IO;

namespace RemoteExecution.Channels
{
	public class NotConnectedException : IOException
	{
		public NotConnectedException(string message)
			: base(message)
		{
		}
	}
}