using System;

namespace RemoteExecution.Core.Dispatchers
{
	/// <summary>
	/// Operation dispatcher interface allowing to register message handlers that would translate incoming messages
	/// into methods calls that would be later executed on registered handler objects.
	/// </summary>
	public interface IOperationDispatcher
	{
		/// <summary>
		/// Registers handler for specified interface. All messages with messageType equal to given interface type name would be handled with this handler.
		/// </summary>
		/// <typeparam name="TInterface">Type of interface, translated to type of handled messages.</typeparam>
		/// <param name="handler">Handler.</param>
		void RegisterHandler<TInterface>(TInterface handler);

		/// <summary>
		/// Registers handler for specified interface. All messages with messageType equal to given interface type name would be handled with this handler.
		/// </summary>
		/// <param name="interfaceType">Type of interface, translated to type of handled messages.</param>
		/// <param name="handler">Handler. Please note that handler has to implement interface specified by <c>interfaceType</c>.</param>
		void RegisterHandler(Type interfaceType, object handler);

		/// <summary>
		/// Unregisters handler for given interface.
		/// </summary>
		/// <typeparam name="TInterface">Type of handler to unregister.</typeparam>
		void UnregisterHandler<TInterface>();

		/// <summary>
		/// Unregisters handler for given interface.
		/// </summary>
		/// <param name="interfaceType">Type of handler to unregister.</param>
		void UnregisterHandler(Type interfaceType);

		/// <summary>
		/// Message dispatcher used by this operation dispatcher
		/// </summary>
		IMessageDispatcher MessageDispatcher { get; }
	}
}
