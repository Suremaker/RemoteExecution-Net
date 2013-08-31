using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Executors
{
	internal class RemoteExecutor : IRemoteExecutor
	{
		private readonly IDuplexChannel _channel;
		private readonly IMessageDispatcher _dispatcher;

		public RemoteExecutor(IDuplexChannel channel, IMessageDispatcher dispatcher)
		{
			_channel = channel;
			_dispatcher = dispatcher;
		}

		#region IRemoteExecutor Members

		public T Create<T>()
		{
			return Create<T>(NoResultMethodExecution.TwoWay);
		}

		public T Create<T>(NoResultMethodExecution noResultMethodExcecution)
		{
			var remoteCallInterceptor = new RemoteCallInterceptor(
				new OneWayRemoteCallInterceptor(_channel, typeof(T).Name),
				new TwoWayRemoteCallInterceptor(_channel, _dispatcher, typeof(T).Name),
				noResultMethodExcecution);

			var factory = new ProxyFactory(typeof(T), remoteCallInterceptor);
			return (T)factory.GetProxy();
		}

		#endregion
	}
}