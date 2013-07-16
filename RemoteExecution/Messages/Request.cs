using System;

namespace RemoteExecution.Messages
{
	[Serializable]
	public class Request : IRequest
	{
		private static readonly object[] EMPTY = new object[0];

		public object[] Args { get; set; }
		public string OperationName { get; set; }
		public bool IsResponseExpected { get; set; }

		public Request()
		{
			IsResponseExpected = true;
		}

		public Request(string id, string interfaceName, string operationName, object[] args, bool isResponseExpected)
		{
			CorrelationId = id;
			GroupId = interfaceName;
			OperationName = operationName;
			Args = args ?? EMPTY;
			IsResponseExpected = isResponseExpected;
		}

		#region IMessage Members

		public string CorrelationId { get; set; }
		public string GroupId { get; set; }

		#endregion
	}
}