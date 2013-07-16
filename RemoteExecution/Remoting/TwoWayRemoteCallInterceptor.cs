using AopAlliance.Intercept;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Handlers;
using RemoteExecution.Messages;

namespace RemoteExecution.Remoting
{
	internal class TwoWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IMessageChannel _channel;
		private readonly string _interfaceName;
		private readonly IOperationDispatcher _operationDispatcher;

		public TwoWayRemoteCallInterceptor(IOperationDispatcher operationDispatcher, IMessageChannel channel, string interfaceName)
		{
			_operationDispatcher = operationDispatcher;
			_channel = channel;
			_interfaceName = interfaceName;
		}

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();

			_operationDispatcher.RegisterResponseHandler(handler);
			try
			{
				_channel.Send(new Request(handler.Id, _interfaceName, invocation.Method.Name, invocation.Arguments, true));
				handler.WaitForResponse();
			}
			finally
			{
				_operationDispatcher.UnregisterResponseHandler(handler);
			}
			return handler.GetValue();
		}

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel);
		}
	}
}