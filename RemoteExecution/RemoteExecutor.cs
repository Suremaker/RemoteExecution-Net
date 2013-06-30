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
			return Create<T>(OneWayMethodExcecution.Synchronized);
		}

		public T Create<T>(OneWayMethodExcecution oneWayMethodExcecution)
		{
			var factory = new ProxyFactory(typeof(T), new RemoteCallInterceptor(_networkConnection.OperationDispatcher, _networkConnection, typeof(T).Name, oneWayMethodExcecution));
			return (T)factory.GetProxy();
		}
	}
}