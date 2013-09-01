using System;
using System.IO;

namespace RemoteExecution.Endpoints
{
	/// <summary>
	/// Server start exception indicating problem with server endpoint start.
	/// </summary>
	public class ServerStartException : IOException
	{
		/// <summary>
		/// Creates exception instance with given message argument.
		/// </summary>
		/// <param name="message">Reason.</param>
		public ServerStartException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates exception instance with given message and inner exception arguments.
		/// </summary>
		/// <param name="message">Reason.</param>
		/// <param name="inner">Inner exception.</param>
		public ServerStartException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}