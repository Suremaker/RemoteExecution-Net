using System.IO;

namespace RemoteExecution.Channels
{
	public class ConnectionException : IOException
	{
		public ConnectionException(string message)
			: base(message)
		{
		}
	}
}