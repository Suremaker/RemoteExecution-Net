using System;
using RemoteExecution.Core.Dispatchers.Handlers;

namespace RemoteExecution.Core.Dispatchers
{
	public class OperationDispatcher : IOperationDispatcher
	{
		public OperationDispatcher()
			: this(new MessageDispatcher())
		{
		}
		protected OperationDispatcher(IMessageDispatcher messageDispatcher)
		{
			MessageDispatcher = messageDispatcher;
			MessageDispatcher.DefaultHandler = new DefaultRequestHandler();
		}

		public void RegisterHandler<TInterface>(TInterface handler)
		{
			RegisterHandler(typeof(TInterface), handler);
		}

		public void RegisterHandler(Type interfaceType, object handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			if (!interfaceType.IsInterface)
				throw new ArgumentException(string.Format("Unable to register handler: '{0}' type is not an interface.", interfaceType.Name));

			if (!interfaceType.IsInstanceOfType(handler))
				throw new ArgumentException(
					string.Format(
						"Unable to register '{0}' handler: it does not implement '{1}' interface.",
						handler.GetType().Name,
						interfaceType.Name));

			MessageDispatcher.Register(new RequestHandler(interfaceType, handler));
		}

		public void UnregisterHandler<TInterface>()
		{
			MessageDispatcher.Unregister(typeof(TInterface).Name);
		}

		public void UnregisterHandler(Type interfaceType)
		{
			MessageDispatcher.Unregister(interfaceType.Name);
		}

		public IMessageDispatcher MessageDispatcher { get; private set; }
	}
}