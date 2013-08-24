using System;

namespace RemoteExecution.Core.TransportLayer
{
	public class UnknownTransportLayerException : InvalidOperationException
	{
		public UnknownTransportLayerException(string message)
			: base(message)
		{
		}
	}
}