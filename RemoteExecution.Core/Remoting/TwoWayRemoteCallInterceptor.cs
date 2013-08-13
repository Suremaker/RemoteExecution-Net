using AopAlliance.Intercept;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Handlers;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Remoting
{
	internal class TwoWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IChannelProvider _channelProvider;
		private readonly string _interfaceName;
		private readonly IMessageDispatcher _messageDispatcher;

		public TwoWayRemoteCallInterceptor(IMessageDispatcher messageDispatcher, IChannelProvider channelProvider, string interfaceName)
		{
			_messageDispatcher = messageDispatcher;
			_channelProvider = channelProvider;
			_interfaceName = interfaceName;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			var handler = CreateResponseHandler();

			_messageDispatcher.Register(handler);
			try
			{
				var outgoingMessageChannel = _channelProvider.GetOutgoingChannel();
				outgoingMessageChannel.Send(new RequestMessage(handler.HandledMessageType, _interfaceName, invocation.Method.Name, invocation.Arguments, true));
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
			return new ResponseHandler(_channelProvider.Id);
		}
	}
}