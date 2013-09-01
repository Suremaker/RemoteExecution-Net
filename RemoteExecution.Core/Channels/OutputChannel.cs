using System;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Output channel abstract class allowing to send messages.
	/// </summary>
	public abstract class OutputChannel : IOutputChannel
	{
		/// <summary>
		/// Serializer used to message serialization/deserialization.
		/// </summary>
		protected readonly IMessageSerializer Serializer;

		/// <summary>
		/// Event fired when channel is closed.
		/// </summary>
		public event Action Closed;

		/// <summary>
		/// Channel constructor.
		/// </summary>
		/// <param name="serializer">Serializer used to serialize message before send.</param>
		protected OutputChannel(IMessageSerializer serializer)
		{
			Serializer = serializer;
			Id = Guid.NewGuid();
		}

		#region IOutputChannel Members

		/// <summary>
		/// Closes channel.
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		/// <summary>
		/// Returns true if channel is opened, otherwise false.
		/// </summary>
		public abstract bool IsOpen { get; }

		/// <summary>
		/// Unique identifier of channel.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Sends given message through this channel.
		/// </summary>
		/// <param name="message">Message to send.</param>
		public void Send(IMessage message)
		{
			if (!IsOpen)
				throw new NotConnectedException("Channel is closed.");

			SendData(Serializer.Serialize(message));
		}

		#endregion

		/// <summary>
		/// Closes channel. 
		/// It should not throw if channel is already closed.
		/// </summary>
		protected abstract void Close();

		/// <summary>
		/// Fires Closed event.
		/// It should be called by implementation when channel has been closed.
		/// </summary>
		protected void FireChannelClosed()
		{
			if (Closed != null)
				Closed();
		}

		/// <summary>
		/// Sends data through channel.
		/// </summary>
		/// <param name="data">Data to send.</param>
		protected abstract void SendData(byte[] data);
	}
}