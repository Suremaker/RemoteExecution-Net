using System;

namespace RemoteExecution.TransportLayer
{
	public class UnknownTransportLayerException : InvalidOperationException
	{
		public UnknownTransportLayerException(string message)
			: base(message)
		{
		}
	}
}