using System;

namespace RemoteExecution.Core.Dispatchers
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
