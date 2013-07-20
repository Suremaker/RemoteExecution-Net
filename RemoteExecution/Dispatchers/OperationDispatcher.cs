using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;

namespace RemoteExecution.Dispatchers
{
	public class OperationDispatcher : IOperationDispatcher
	{
		private readonly ConcurrentDictionary<string, IHandler> _handlers = new ConcurrentDictionary<string, IHandler>();

		public OperationDispatcher()
		{
		}

		public OperationDispatcher(IDictionary<Type, object> requestHandlers)
		{
			foreach (var requestHandler in requestHandlers)
				RegisterRequestHandler(requestHandler.Key, requestHandler.Value);
		}

		#region IOperationDispatcher Members

		public IOperationDispatcher RegisterRequestHandler<TInterface>(TInterface handler)
		{
			return RegisterRequestHandler(typeof(TInterface), handler);
		}

		public IOperationDispatcher RegisterResponseHandler(IResponseHandler handler)
		{
			return RegisterHandler(handler);
		}

		public IOperationDispatcher UnregisterResponseHandler(IResponseHandler handler)
		{
			IHandler hnd;
			_handlers.TryRemove(handler.Id, out hnd);
			return this;
		}

		public void DispatchAbortResponsesFor(IMessageChannel originChannel, string message)
		{
			var handlers = _handlers.Values.OfType<IResponseHandler>().Where(h => h.TargetChannel == originChannel).ToArray();
			foreach (var responseHandler in handlers)
				responseHandler.Handle(new ExceptionResponse(responseHandler.Id, typeof(OperationAbortedException), message), originChannel);
		}

		public IOperationDispatcher UnregisterRequestHandler<TInterface>()
		{
			IHandler handler;
			_handlers.TryRemove(typeof(TInterface).Name, out handler);
			return this;
		}

		public void Dispatch(IMessage msg, IMessageChannel originChannel)
		{
			IHandler handler;
			if (_handlers.TryGetValue(msg.GroupId, out handler))
				handler.Handle(msg, originChannel);
			else
				HandleUndefinedType(msg, originChannel);
		}

		public IOperationDispatcher RegisterRequestHandler(Type interfaceType, object handler)
		{
			if (!interfaceType.IsInterface)
				throw new ArgumentException(string.Format("Unable to register handler: {0} type is not an interface.", interfaceType.Name));

			if (!interfaceType.IsInstanceOfType(handler))
				throw new ArgumentException(
					string.Format(
						"Unable to register {0} handler: it does not implement {1} interface.",
						handler.GetType().Name,
						interfaceType.Name));

			return RegisterHandler(new RequestHandler(interfaceType, handler));
		}

		#endregion

		private void HandleUndefinedType(IMessage msg, IMessageChannel messageChannel)
		{
			if (!(msg is IRequest))
				return;
			string message = string.Format("No handler is defined for {0} type.", msg.GroupId);
			messageChannel.Send(new ExceptionResponse(msg.CorrelationId, typeof(InvalidOperationException), message));
		}

		private IOperationDispatcher RegisterHandler(IHandler handler)
		{
			_handlers.AddOrUpdate(handler.Id, k => handler, (k, v) => handler);
			return this;
		}
	}
}