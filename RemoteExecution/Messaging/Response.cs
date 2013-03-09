using System;

namespace RemoteExecution.Messaging
{
	[Serializable]
	public class Response : IMessage
	{
		public object Value { get; set; }

		public Response(string id, object value)
		{
			CorrelationId = id;
			Value = value;
		}

		public Response() { }

		#region IMessage Members

		public string CorrelationId { get; set; }
		public string GroupId { get { return CorrelationId; } }

		#endregion
	}
}