using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RemoteExecution.Core.Dispatchers
{
	public class MessageDispatcher : IMessageDispatcher
	{
		private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<IMessageHandler, IMessageHandler>> _handlerGroups = new ConcurrentDictionary<Guid, ConcurrentDictionary<IMessageHandler, IMessageHandler>>();
		private readonly ConcurrentDictionary<string, IMessageHandler> _messageTypeHandlers = new ConcurrentDictionary<string, IMessageHandler>();

		#region IMessageDispatcher Members

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

		public void Unregister(string messageType)
		{
			IMessageHandler removedHandler;
			if (_messageTypeHandlers.TryRemove(messageType, out removedHandler))
				RemoveHandlerFromGroup(removedHandler);
		}

		public void Dispatch(IMessage message)
		{
			IMessageHandler handler;
			if (!_messageTypeHandlers.TryGetValue(message.MessageType, out handler))
				handler = DefaultHandler;

			if (handler == null)
				throw new InvalidOperationException(string.Format("Unable to dispatch message of type '{0}': no suitable handlers were found.", message.MessageType));

			handler.Handle(message);
		}

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