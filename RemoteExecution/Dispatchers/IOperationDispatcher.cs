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
		void RegisterRequestHandler<TInterface>(TInterface handler);
		void RegisterRequestHandler(Type interfaceType, object handler);

		void RegisterResponseHandler(IResponseHandler handler);
		void UnregisterRequestHandler<TInterface>();
		void UnregisterResponseHandler(IResponseHandler handler);
	}
}