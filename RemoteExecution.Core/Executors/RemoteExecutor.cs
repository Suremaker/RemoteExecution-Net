using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution.Core.Executors
{
	internal class RemoteExecutor : IRemoteExecutor
	{
		private readonly IRemoteConnection _connection;

		public RemoteExecutor(IRemoteConnection connection)
		{
			_connection = connection;
		}

		#region IRemoteExecutor Members

		public T Create<T>()
		{
			return Create<T>(NoResultMethodExecution.TwoWay);
		}

		public T Create<T>(NoResultMethodExecution noResultMethodExcecution)
		{
			var remoteCallInterceptor = new RemoteCallInterceptor(
				new OneWayRemoteCallInterceptor(_connection, typeof(T).Name),
				new TwoWayRemoteCallInterceptor(_connection, typeof(T).Name),
				noResultMethodExcecution);

			var factory = new ProxyFactory(typeof(T), remoteCallInterceptor);
			return (T)factory.GetProxy();
		}

		#endregion
	}
}