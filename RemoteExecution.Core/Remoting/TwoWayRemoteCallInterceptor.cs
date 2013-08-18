using AopAlliance.Intercept;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Remoting
{
	internal class TwoWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IOutputChannel _channel;
		private readonly IMessageDispatcher _messageDispatcher;
		private readonly string _interfaceName;

		public TwoWayRemoteCallInterceptor(IOutputChannel channel, IMessageDispatcher messageDispatcher, string interfaceName)
		{
			_channel = channel;
			_messageDispatcher = messageDispatcher;
			_interfaceName = interfaceName;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();

			_messageDispatcher.Register(handler);
			try
			{
				_channel.Send(new RequestMessage(handler.HandledMessageType, _interfaceName, invocation.Method.Name, invocation.Arguments, true));
				handler.WaitForResponse();
			}
			finally
			{
				_messageDispatcher.Unregister(handler.HandledMessageType);
			}
			return handler.GetValue();
		}

		#endregion

		protected virtual IResponseHandler CreateResponseHandler()
		{
			return new ResponseHandler(_channel.Id);
		}
	}
}