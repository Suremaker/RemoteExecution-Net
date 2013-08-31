using System;
using RemoteExecution.Dispatchers.Handlers;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Dispatchers
{
	/// <summary>
	/// Message dispatcher interface allowing to register message handlers and dispatch messages.
	/// </summary>
	public interface IMessageDispatcher
	{
		/// <summary>
		/// Default message handler which is used if there is no registered handler for dispatched message.
		/// </summary>
		IMessageHandler DefaultHandler { get; set; }

		/// <summary>
		/// Dispatches given message to one of registered handlers.
		/// </summary>
		/// <param name="message">Message to dispatch.</param>
		void Dispatch(IMessage message);

		/// <summary>
		/// Dispatches given message to all handlers belonging to given handler group id.
		/// </summary>
		/// <param name="handlerGroupId">Handler group id.</param>
		/// <param name="message">Message to dispatch.</param>
		void GroupDispatch(Guid handlerGroupId, IMessage message);

		/// <summary>
		/// Registers message handler.
		/// </summary>
		/// <param name="handler">Handler.</param>
		void Register(IMessageHandler handler);

		/// <summary>
		/// Unregisters message handler for given message type.
		/// </summary>
		/// <param name="messageType">Message type.</param>
		void Unregister(string messageType);
	}
}
