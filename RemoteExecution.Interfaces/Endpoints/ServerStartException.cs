using System;
using System.IO;

namespace RemoteExecution.Endpoints
{
	public class ServerStartException : IOException
	{
		public ServerStartException(string message)
			: base(message)
		{
		}

		public ServerStartException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}