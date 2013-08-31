using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	public abstract class DuplexChannel : OutputChannel, IDuplexChannel
	{
		public event Action<IMessage> Received;

		protected DuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

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