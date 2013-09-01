using System;
using RemoteExecution.Dispatchers.Handlers;

namespace RemoteExecution.Dispatchers
{
	/// <summary>
	/// Operation dispatcher class allowing to register message handlers that would translate incoming messages
	/// into methods calls that would be later executed on registered handler objects.
	/// </summary>
	public class OperationDispatcher : IOperationDispatcher
	{
		/// <summary>
		/// Creates instance of operation dispatcher with new instance of <see cref="MessageDispatcher"/> class.
		/// </summary>
		public OperationDispatcher()
			: this(new MessageDispatcher())
		{
		}

		/// <summary>
		/// Creates instance of operation dispatcher with given message dispatcher object.
		/// </summary>
		/// <param name="messageDispatcher">Message dispatcher.</param>
		protected OperationDispatcher(IMessageDispatcher messageDispatcher)
		{
			MessageDispatcher = messageDispatcher;
			MessageDispatcher.DefaultHandler = new DefaultRequestHandler();
		}

		#region IOperationDispatcher Members

		/// <summary>
		/// Registers handler for specified interface. All messages with messageType equal to given interface type name would be handled with this handler.
		/// </summary>
		/// <typeparam name="TInterface">Type of interface, translated to type of handled messages.</typeparam>
		/// <param name="handler">Handler.</param>
		public IOperationDispatcher RegisterHandler<TInterface>(TInterface handler)
		{
			RegisterHandler(typeof(TInterface), handler);
			return this;
		}

		/// <summary>
		/// Registers handler for specified interface. All messages with messageType equal to given interface type name would be handled with this handler.
		/// </summary>
		/// <param name="interfaceType">Type of interface, translated to type of handled messages.</param>
		/// <param name="handler">Handler. Please note that handler has to implement interface specified by <c>interfaceType</c>.</param>
		public IOperationDispatcher RegisterHandler(Type interfaceType, object handler)
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
			return this;
		}

		/// <summary>
		/// Unregisters handler for given interface.
		/// </summary>
		/// <typeparam name="TInterface">Type of handler to unregister.</typeparam>
		public IOperationDispatcher UnregisterHandler<TInterface>()
		{
			MessageDispatcher.Unregister(typeof(TInterface).Name);
			return this;
		}

		/// <summary>
		/// Unregisters handler for given interface.
		/// </summary>
		/// <param name="interfaceType">Type of handler to unregister.</param>
		public IOperationDispatcher UnregisterHandler(Type interfaceType)
		{
			MessageDispatcher.Unregister(interfaceType.Name);
			return this;
		}

		/// <summary>
		/// Message dispatcher used by this operation dispatcher
		/// </summary>
		public IMessageDispatcher MessageDispatcher { get; private set; }

		#endregion
	}
}