using System.IO;

namespace RemoteExecution.Connections
{
	public class NotConnectedException : IOException
	{
		public NotConnectedException(string message)
			: base(message)
		{
		}
	}
}