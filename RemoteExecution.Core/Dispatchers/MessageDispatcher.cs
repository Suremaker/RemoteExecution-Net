using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RemoteExecution.Dispatchers.Handlers;
using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Dispatchers
{
	/// <summary>
	/// Message dispatcher class allowing to register message handlers and dispatch messages.
	/// </summary>
	public class MessageDispatcher : IMessageDispatcher
	{
		private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<IMessageHandler, IMessageHandler>> _handlerGroups = new ConcurrentDictionary<Guid, ConcurrentDictionary<IMessageHandler, IMessageHandler>>();
		private readonly ConcurrentDictionary<string, IMessageHandler> _messageTypeHandlers = new ConcurrentDictionary<string, IMessageHandler>();

		#region IMessageDispatcher Members

		/// <summary>
		/// Registers message handler.
		/// </summary>
		/// <param name="handler">Handler.</param>
		public void Register(IMessageHandler handler)
		{
			if (handler.HandledMessageType == null)
				throw new ArgumentException("Handler does not have HandledMessageType specified.", "handler");
			if (handler.HandlerGroupId == Guid.Empty)
				throw new ArgumentException("Handler does not have HandlerGroupId specified.", "handler");

			if (!_messageTypeHandlers.TryAdd(handler.HandledMessageType, handler))
				throw new ArgumentException(string.Format("Unable to register handler for message type '{0}': only one handler could be registered for given message type.", handler.HandledMessageType), "handler");

			_handlerGroups.GetOrAdd(handler.HandlerGroupId, key => new ConcurrentDictionary<IMessageHandler, IMessageHandler>())
				.TryAdd(handler, handler);
		}

		/// <summary>
		/// Unregisters message handler for given message type.
		/// </summary>
		/// <param name="messageType">Message type.</param>
		public void Unregister(string messageType)
		{
			IMessageHandler removedHandler;
			if (_messageTypeHandlers.TryRemove(messageType, out removedHandler))
				RemoveHandlerFromGroup(removedHandler);
		}

		/// <summary>
		/// Dispatches given message to one of registered handlers.
		/// </summary>
		/// <param name="message">Message to dispatch.</param>
		public void Dispatch(IMessage message)
		{
			IMessageHandler handler;
			if (!_messageTypeHandlers.TryGetValue(message.MessageType, out handler))
				handler = DefaultHandler;

			if (handler == null)
				throw new InvalidOperationException(string.Format("Unable to dispatch message of type '{0}': no suitable handlers were found.", message.MessageType));

			handler.Handle(message);
		}

		/// <summary>
		/// Dispatches given message to all handlers belonging to given handler group id.
		/// </summary>
		/// <param name="handlerGroupId">Handler group id.</param>
		/// <param name="message">Message to dispatch.</param>
		public void GroupDispatch(Guid handlerGroupId, IMessage message)
		{
			// ReSharper disable PossibleMultipleEnumeration
			ConcurrentDictionary<IMessageHandler, IMessageHandler> handlerGroup;

			IEnumerable<IMessageHandler> handlers = Enumerable.Empty<IMessageHandler>();

			if (_handlerGroups.TryGetValue(handlerGroupId, out handlerGroup))
				handlers = handlerGroup.Keys;

			if (!handlers.Any())
				handlers = Enumerable.Repeat(DefaultHandler, 1).Where(h => h != null);

			if (!handlers.Any())
				throw new InvalidOperationException(string.Format("Unable to dispatch message to group '{0}': no suitable handlers were found.", handlerGroupId));

			foreach (var messageHandler in handlers)
				messageHandler.Handle(message);
			// ReSharper restore PossibleMultipleEnumeration
		}

		/// <summary>
		/// Default message handler which is used if there is no registered handler for dispatched message.
		/// </summary>
		public IMessageHandler DefaultHandler { get; set; }

		#endregion

		private void RemoveHandlerFromGroup(IMessageHandler removedHandler)
		{
			ConcurrentDictionary<IMessageHandler, IMessageHandler> handlerGroup;
			if (_handlerGroups.TryGetValue(removedHandler.HandlerGroupId, out handlerGroup))
				handlerGroup.TryRemove(removedHandler, out removedHandler);
		}
	}
}