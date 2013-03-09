using System;

namespace RemoteExecution.Messaging
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

		public object Value
		{
			get { throw (Exception)Activator.CreateInstance(Type.GetType(ExceptionType, true), Message); }
		}

		#region IMessage Members

		public string CorrelationId { get; set; }
		public string GroupId { get { return CorrelationId; } }

		#endregion
	}
}