using System;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;

namespace RemoteExecution.Dispatching
{
	public interface IOperationDispatcher
	{
		void RegisterRequestHandler<TInterface>(TInterface handler);
		void RegisterRequestHandler(Type interfaceType, object handler);
		void UnregisterRequestHandler<TInterface>();

		void RegisterResponseHandler(IResponseHandler handler);
		void UnregisterResponseHandler(IResponseHandler handler);
		void DispatchAbortResponses(IMessageChannel originChannel, string message);
		void Dispatch(IMessage msg, IMessageChannel originChannel);
	}
}