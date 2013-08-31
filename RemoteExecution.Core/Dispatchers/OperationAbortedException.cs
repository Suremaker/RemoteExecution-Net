using System;

namespace RemoteExecution.Dispatchers
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
