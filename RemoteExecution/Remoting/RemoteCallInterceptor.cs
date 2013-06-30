using System;
using AopAlliance.Intercept;
using RemoteExecution.Dispatching;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;

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

	internal class OneWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IMessageChannel _channel;
		private readonly string _interfaceName;

		public OneWayRemoteCallInterceptor(IMessageChannel channel, string interfaceName)
		{
			_channel = channel;
			_interfaceName = interfaceName;
		}

		public object Invoke(IMethodInvocation invocation)
		{
			_channel.Send(new Request(Guid.NewGuid().ToString(), _interfaceName, invocation.Method.Name, invocation.Arguments, false));
			return null;
		}
	}

	internal class RemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IMethodInterceptor _oneWayInterceptor;
		private readonly IMethodInterceptor _twoWayInterceptor;
		private readonly NoResultMethodExecution _noResultMethodExecution;

		public RemoteCallInterceptor(IMethodInterceptor oneWayInterceptor, IMethodInterceptor twoWayInterceptor, NoResultMethodExecution noResultMethodExecution)
		{
			_oneWayInterceptor = oneWayInterceptor;
			_twoWayInterceptor = twoWayInterceptor;
			_noResultMethodExecution = noResultMethodExecution;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			if (_noResultMethodExecution == NoResultMethodExecution.OneWay && invocation.Method.ReturnType == typeof(void))
				return _oneWayInterceptor.Invoke(invocation);
			return _twoWayInterceptor.Invoke(invocation);
		}

		#endregion
	}
}