using System.IO;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Not connected exception indicating problems like trying to use connection which is not opened.
	/// </summary>
	public class NotConnectedException : IOException
	{
		/// <summary>
		/// Creates exception instance with given message argument.
		/// </summary>
		/// <param name="message"></param>
		public NotConnectedException(string message)
			: base(message)
		{
		}
	}
}