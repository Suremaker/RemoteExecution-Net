using System;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Dispatchers.Handlers
{
	/// <summary>
	/// Interface for message handler.
	/// </summary>
	public interface IMessageHandler
	{
		/// <summary>
		/// Type of messages handled by this handler.
		/// </summary>
		string HandledMessageType { get; }

		/// <summary>
		/// Identifier of handler group.
		/// </summary>
		Guid HandlerGroupId { get; }

		/// <summary>
		/// Handles given message.
		/// </summary>
		/// <param name="message">Message to handle.</param>
		void Handle(IMessage message);
	}
}