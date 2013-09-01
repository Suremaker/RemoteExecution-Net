using System.IO;

namespace RemoteExecution.Dispatchers
{
	/// <summary>
	/// Connection open exception indicating that remote operation execution has been aborted (for example, because connection has been closed before remote operation finished).
	/// </summary>
	public class OperationAbortedException : IOException
	{
		/// <summary>
		/// Creates exception instance with no detail message.
		/// </summary>
		public OperationAbortedException()
		{
		}

		/// <summary>
		/// Creates exception instance with given message argument.
		/// </summary>
		/// <param name="message">Reason.</param>
		public OperationAbortedException(string message)
			: base(message)
		{
		}
	}
}
