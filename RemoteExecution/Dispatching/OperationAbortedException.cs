using System;

namespace RemoteExecution.Dispatching
{
	public class OperationAbortedException : Exception
	{
		public OperationAbortedException()
		{
		}

		public OperationAbortedException(string reason)
			: base(reason)
		{
		}
	}
}
