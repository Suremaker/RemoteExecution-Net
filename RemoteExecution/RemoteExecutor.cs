using RemoteExecution.Endpoints;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution
{
	public class RemoteExecutor : IRemoteExecutor
	{
		private readonly INetworkConnection _networkConnection;

		public RemoteExecutor(INetworkConnection networkConnection)
		{
			_networkConnection = networkConnection;
		}

		public T Create<T>()
		{
			return Create<T>(NoResultMethodExecution.TwoWay);
		}

		public T Create<T>(NoResultMethodExecution noResultMethodExcecution)
		{
			var remoteCallInterceptor = new RemoteCallInterceptor(
				new OneWayRemoteCallInterceptor(_networkConnection, typeof(T).Name),
				new TwoWayRemoteCallInterceptor(_networkConnection.OperationDispatcher, _networkConnection, typeof(T).Name),
				noResultMethodExcecution);

			var factory = new ProxyFactory(typeof(T), remoteCallInterceptor);
			return (T)factory.GetProxy();
		}
	}
}