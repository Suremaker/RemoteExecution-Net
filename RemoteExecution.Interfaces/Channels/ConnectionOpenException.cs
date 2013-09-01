using System.IO;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Connection open exception indicating problem with opening connection.
	/// </summary>
	public class ConnectionOpenException : IOException
	{
		/// <summary>
		/// Creates exception instance with given message argument.
		/// </summary>
		/// <param name="message">Reason.</param>
		public ConnectionOpenException(string message)
			: base(message)
		{
		}
	}
}