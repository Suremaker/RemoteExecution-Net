using System;
using RemoteExecution.Core.Channels;

namespace RemoteExecution.Core.Dispatchers.Messages
{
	[Serializable]
	internal class RequestMessage : IRequestMessage
	{
		private static readonly object[] EMPTY = new object[0];
		[NonSerialized]
		private IChannelProvider _channelProvider;

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

		#region IRequest Members

		public object[] Args { get; set; }
		public string MethodName { get; set; }
		public bool IsResponseExpected { get; set; }

		public string CorrelationId { get; set; }
		public string MessageType { get; set; }

		public IChannelProvider ChannelProvider
		{
			get { return _channelProvider; }
			set { _channelProvider = value; }
		}

		#endregion
	}
}
