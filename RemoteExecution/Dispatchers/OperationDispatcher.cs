using System;
using System.Collections.Concurrent;
using System.Linq;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;

namespace RemoteExecution.Dispatchers
{
	public class OperationDispatcher : IOperationDispatcher
	{
		private readonly ConcurrentDictionary<string, IHandler> _handlers = new ConcurrentDictionary<string, IHandler>();

		#region IOperationDispatcher Members

		public void RegisterRequestHandler<TInterface>(TInterface handler)
		{
			RegisterRequestHandler(typeof(TInterface), handler);
		}

		public void RegisterResponseHandler(IResponseHandler handler)
		{
			RegisterHandler(handler);
		}

		private void RegisterHandler(IHandler handler)
		{
			_handlers.AddOrUpdate(handler.Id, k => handler, (k, v) => handler);
		}

		public void UnregisterResponseHandler(IResponseHandler handler)
		{
			IHandler hnd;
			_handlers.TryRemove(handler.Id, out hnd);
		}

		public void DispatchAbortResponses(IMessageChannel originChannel, string message)
		{
			var handlers = _handlers.Values.OfType<IResponseHandler>().Where(h => h.TargetChannel == originChannel).ToArray();
			foreach (var responseHandler in handlers)
				responseHandler.Handle(new ExceptionResponse(responseHandler.Id, typeof(OperationAbortedException), message), originChannel);
		}

		public void UnregisterRequestHandler<TInterface>()
		{
			IHandler handler;
			_handlers.TryRemove(typeof(TInterface).Name, out handler);
		}

		public void Dispatch(IMessage msg, IMessageChannel originChannel)
		{
			IHandler handler;
			if (_handlers.TryGetValue(msg.GroupId, out handler))
				handler.Handle(msg, originChannel);
			else
				HandleUndefinedType(msg, originChannel);
		}

		#endregion

		public void RegisterRequestHandler(Type interfaceType, object handler)
		{
			if (!interfaceType.IsInstanceOfType(handler))
				throw new ArgumentException(
					string.Format(
						"Unable to register {0} handler: it does not implement {1} interface.",
						handler.GetType().Name,
						interfaceType.Name));

			RegisterHandler(new RequestHandler(interfaceType, handler));
		}

		private void HandleUndefinedType(IMessage msg, IMessageChannel messageChannel)
		{
			if (!(msg is Request))
				return;
			string message = string.Format("No handler is defined for {0} type.", msg.GroupId);
			messageChannel.Send(new ExceptionResponse(msg.CorrelationId, typeof(InvalidOperationException), message));
		}
	}
}