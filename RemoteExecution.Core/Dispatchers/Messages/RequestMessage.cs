﻿using System;
using RemoteExecution.Channels;

namespace RemoteExecution.Dispatchers.Messages
{
	[Serializable]
	internal class RequestMessage : IRequestMessage
	{
		private static readonly object[] EMPTY = new object[0];
		[NonSerialized]
		private IOutputChannel _channel;

		public RequestMessage()
		{
			IsResponseExpected = true;
		}

		public RequestMessage(string id, string interfaceName, string methodName, object[] args, bool isResponseExpected)
		{
			CorrelationId = id;
			MessageType = interfaceName;
			MethodName = methodName;
			Args = args ?? EMPTY;
			IsResponseExpected = isResponseExpected;
		}

		#region IRequestMessage Members

		public object[] Args { get; set; }
		public string MethodName { get; set; }
		public bool IsResponseExpected { get; set; }

		public string CorrelationId { get; set; }
		public string MessageType { get; set; }

		public IOutputChannel Channel
		{
			get { return _channel; }
			set { _channel = value; }
		}

		#endregion
	}
}
