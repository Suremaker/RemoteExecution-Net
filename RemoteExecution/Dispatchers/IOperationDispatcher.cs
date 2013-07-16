using System;
using RemoteExecution.Channels;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;

namespace RemoteExecution.Dispatchers
{
	public interface IOperationDispatcher
	{
		void RegisterRequestHandler<TInterface>(TInterface handler);
		void RegisterRequestHandler(Type interfaceType, object handler);
		void UnregisterRequestHandler<TInterface>();

		void RegisterResponseHandler(IResponseHandler handler);
		void UnregisterResponseHandler(IResponseHandler handler);

		void DispatchAbortResponsesFor(IMessageChannel originChannel, string message);
		void Dispatch(IMessage msg, IMessageChannel originChannel);
	}
}