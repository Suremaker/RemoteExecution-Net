using System;

namespace RemoteExecution.Core.Dispatchers.Messages
{
	[Serializable]
	internal class ResponseMessage : IResponseMessage
	{
		public ResponseMessage(string id, object value)
		{
			CorrelationId = id;
			Value = value;
		}

		public ResponseMessage() { }

		#region IResponseMessage Members

		public object Value { get; set; }

		public string CorrelationId { get; set; }
		public string MessageType { get { return CorrelationId; } }

		#endregion
	}
}