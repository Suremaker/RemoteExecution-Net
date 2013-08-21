using System.IO;

namespace RemoteExecution.Core.Channels
{
	public class ConnectionException : IOException
	{
		public ConnectionException(string message)
			: base(message)
		{
		}
	}
}