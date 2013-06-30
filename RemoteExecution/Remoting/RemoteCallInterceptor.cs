using System;
using AopAlliance.Intercept;
using RemoteExecution.Dispatching;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;

namespace RemoteExecution.Remoting
{
	internal class RemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IMessageChannel _channel;
		private readonly string _interfaceName;
		private readonly OneWayMethodExcecution _oneWayMethodExcecution;
		private readonly IOperationDispatcher _operationDispatcher;

		public RemoteCallInterceptor(IOperationDispatcher operationDispatcher, IMessageChannel channel, string interfaceName, OneWayMethodExcecution oneWayMethodExcecution)
		{
			_operationDispatcher = operationDispatcher;
			_channel = channel;
			_interfaceName = interfaceName;
			_oneWayMethodExcecution = oneWayMethodExcecution;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			if (_oneWayMethodExcecution == OneWayMethodExcecution.Asynchronized && invocation.Method.ReturnType == typeof(void))
				return InvokeNoWait(invocation);
			return InvokeWithWait(invocation);
		}

		private object InvokeWithWait(IMethodInvocation invocation)
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

		private object InvokeNoWait(IMethodInvocation invocation)
		{
			_channel.Send(new Request(Guid.NewGuid().ToString(), _interfaceName, invocation.Method.Name, invocation.Arguments, false));
			return null;
		}

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel);
		}

		#endregion
	}
}