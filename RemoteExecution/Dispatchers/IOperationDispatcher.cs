using System;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;

namespace RemoteExecution.Dispatchers
{
	public interface IOperationDispatcher
	{
		void Dispatch(IMessage msg, IMessageChannel originChannel);
		void DispatchAbortResponsesFor(IMessageChannel originChannel, string message);
		IOperationDispatcher RegisterRequestHandler<TInterface>(TInterface handler);
		IOperationDispatcher RegisterRequestHandler(Type interfaceType, object handler);

		IOperationDispatcher RegisterResponseHandler(IResponseHandler handler);
		IOperationDispatcher UnregisterRequestHandler<TInterface>();
		IOperationDispatcher UnregisterResponseHandler(IResponseHandler handler);
	}
}