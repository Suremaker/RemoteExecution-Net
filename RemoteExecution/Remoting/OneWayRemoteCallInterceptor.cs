using System;
using AopAlliance.Intercept;
using RemoteExecution.Channels;
using RemoteExecution.Messages;

namespace RemoteExecution.Remoting
{
	internal class OneWayRemoteCallInterceptor : IMethodInterceptor
	{
		private readonly IOutgoingMessageChannel _channel;
		private readonly string _interfaceName;

		public OneWayRemoteCallInterceptor(IOutgoingMessageChannel channel, string interfaceName)
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
}