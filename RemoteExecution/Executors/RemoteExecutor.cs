using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Executors
{
	internal class RemoteExecutor : IRemoteExecutor
	{
		private readonly IOperationDispatcher _dispatcher;
		private readonly IMessageChannel _channel;

		public RemoteExecutor(IOperationDispatcher dispatcher, IMessageChannel channel)
		{
			_dispatcher = dispatcher;
			_channel = channel;
		}

		public T Create<T>()
		{
			return Create<T>(NoResultMethodExecution.TwoWay);
		}

		public T Create<T>(NoResultMethodExecution noResultMethodExcecution)
		{
			var remoteCallInterceptor = new RemoteCallInterceptor(
				new OneWayRemoteCallInterceptor(_channel, typeof(T).Name),
				new TwoWayRemoteCallInterceptor(_dispatcher, _channel, typeof(T).Name),
				noResultMethodExcecution);

			var factory = new ProxyFactory(typeof(T), remoteCallInterceptor);
			return (T)factory.GetProxy();
		}
	}
}