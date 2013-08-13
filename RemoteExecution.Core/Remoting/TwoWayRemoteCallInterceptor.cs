using AopAlliance.Intercept;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Remoting
{
	internal class TwoWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IRemoteConnection _connection;
		private readonly string _interfaceName;

		public TwoWayRemoteCallInterceptor(IRemoteConnection connection, string interfaceName)
		{
			_connection = connection;
			_interfaceName = interfaceName;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();
			var messageDispatcher = _connection.Dispatcher.MessageDispatcher;

			messageDispatcher.Register(handler);
			try
			{
				_connection.GetOutgoingChannel().Send(new RequestMessage(handler.HandledMessageType, _interfaceName, invocation.Method.Name, invocation.Arguments, true));
				handler.WaitForResponse();
			}
			finally
			{
				messageDispatcher.Unregister(handler.HandledMessageType);
			}
			return handler.GetValue();
		}

		#endregion

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_connection.Id);
		}
	}
}