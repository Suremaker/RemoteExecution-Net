using System;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;
using RemoteExecution.Core.Serializers;

namespace RemoteExecution.Core.Channels
{
	public abstract class DuplexChannel : OutputChannel, IDuplexChannel
	{
		protected DuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

		public event Action<IMessage> Received;
		protected void OnReceive(byte[] data)
		{
			if (Received != null)
				Received(DeserializeMessage(data));
		}

		private IMessage DeserializeMessage(byte[] data)
		{
			var message = Serializer.Deserialize(data);
			var request = message as IRequestMessage;
			if (request != null)
				request.Channel = this;
			return message;
		}
	}
}