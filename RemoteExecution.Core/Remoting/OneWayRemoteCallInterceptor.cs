using System;
using AopAlliance.Intercept;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Remoting
{
	internal class OneWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IChannelProvider _channelProvider;
		private readonly string _interfaceName;

		public OneWayRemoteCallInterceptor(IChannelProvider channelProvider, string interfaceName)
		{
			_channelProvider = channelProvider;
			_interfaceName = interfaceName;
		}

		#region IMethodInterceptor Members

		public object Invoke(IMethodInvocation invocation)
		{
			_channelProvider.GetOutgoingChannel().Send(new RequestMessage(Guid.NewGuid().ToString(), _interfaceName, invocation.Method.Name, invocation.Arguments, false));
			return null;
		}

		#endregion
	}
}