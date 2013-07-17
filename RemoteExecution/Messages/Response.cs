using System;

namespace RemoteExecution.Messages
{
	[Serializable]
	public class Response : IResponse
	{
		public Response(string id, object value)
		{
			CorrelationId = id;
			Value = value;
		}

		public Response() { }

		#region IResponse Members

		public object Value { get; set; }

		public string CorrelationId { get; set; }
		public string GroupId { get { return CorrelationId; } }

		#endregion
	}
}