using System;
using RemoteExecution.Dispatching;
using RemoteExecution.Messaging;

namespace RemoteExecution.Endpoints
{
	public interface INetworkConnection:IMessageSender,IDisposable
	{
		bool IsOpen { get; }
		IOperationDispatcher OperationDispatcher { get; }
	}
}