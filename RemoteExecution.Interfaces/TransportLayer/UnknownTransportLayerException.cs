using System;

namespace RemoteExecution.TransportLayer
{
	/// <summary>
	/// Unknown transport layer exception indicating problem with uri scheme used for creation of client channel or server connection listener.
	/// </summary>
	public class UnknownTransportLayerException : InvalidOperationException
	{
		/// <summary>
		/// Creates exception instance with given message argument.
		/// </summary>
		/// <param name="message">Reason.</param>
		public UnknownTransportLayerException(string message)
			: base(message)
		{
		}
	}
}