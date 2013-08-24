using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Core.Channels;
using RemoteExecution.Core.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.Executors
{
	internal class BroadcastRemoteExecutor : IBroadcastRemoteExecutor
	{
		private readonly IBroadcastChannel _broadcastChannel;

		public BroadcastRemoteExecutor(IBroadcastChannel broadcastChannel)
		{
			_broadcastChannel = broadcastChannel;
		}

		#region IBroadcastRemoteExecutor Members

		public T Create<T>()
		{
			var interfaceType = typeof(T);

			VerifyInterfaceMethods(interfaceType, interfaceType.Name);

			return (T)new ProxyFactory(interfaceType, new OneWayRemoteCallInterceptor(_broadcastChannel, interfaceType.Name)).GetProxy();
		}

		#endregion

		private static void VerifyInterfaceMethods(Type interfaceType, string name)
		{
			if (interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.ReturnType != typeof(void)))
				throw new InvalidOperationException(string.Format("{0} interface cannot be used for broadcasting because some of its methods returns result.", name));

			foreach (var baseInterface in interfaceType.GetInterfaces())
				VerifyInterfaceMethods(baseInterface, name);
		}
	}
}