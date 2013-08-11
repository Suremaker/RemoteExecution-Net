using System;

namespace RemoteExecution.Core.Dispatchers.Messages
{
	[Serializable]
	internal class ExceptionResponseMessage : IResponseMessage
	{
		public string ExceptionType { get; set; }
		public string Message { get; set; }

		public ExceptionResponseMessage()
		{
		}

		public ExceptionResponseMessage(string id, Type exceptionType, string message)
		{
			ExceptionType = exceptionType.AssemblyQualifiedName;
			Message = message;
			CorrelationId = id;
		}

		#region IResponse Members

		public object Value
		{
			get { throw (Exception)Activator.CreateInstance(Type.GetType(ExceptionType, true), Message); }
		}

		public string CorrelationId { get; set; }
		public string MessageType { get { return CorrelationId; } }
		#endregion
	}
}