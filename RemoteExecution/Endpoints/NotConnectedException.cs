using System.IO;

namespace RemoteExecution.Endpoints
{
	public class NotConnectedException : IOException
	{
		public NotConnectedException(string message)
			: base(message)
		{
		}
	}
}