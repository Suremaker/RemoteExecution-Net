using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Duplex channel class allowing to send and receive messages.
	/// </summary>
	public abstract class DuplexChannel : OutputChannel, IDuplexChannel
	{
		/// <summary>
		/// Fires when new message has been received through this channel.
		/// </summary>
		public event Action<IMessage> Received;

		/// <summary>
		/// Channel constructor.
		/// </summary>
		/// <param name="serializer">Serializer used to serialize message before send and deserialize it after receive.</param>
		protected DuplexChannel(IMessageSerializer serializer)
			: base(serializer)
		{
		}

		/// <summary>
		/// Method that should be called by implementation when message is received.
		/// It deserializes it and fires Received event.
		/// </summary>
		/// <param name="data"></param>
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