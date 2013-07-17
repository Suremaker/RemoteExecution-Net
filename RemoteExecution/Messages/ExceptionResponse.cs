using System;

namespace RemoteExecution.Messages
{
	[Serializable]
	public class ExceptionResponse : IResponse
	{
		public string ExceptionType { get; private set; }
		public string Message { get; private set; }

		public ExceptionResponse(string id, Type exceptionType, string message)
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
		public string GroupId { get { return CorrelationId; } }

		#endregion
	}
}