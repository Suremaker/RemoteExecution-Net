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
			return Create<T>(ExecutionMode.AlwaysWaitForResponse);
		}

		public T Create<T>(ExecutionMode executionMode)
		{
			var factory = new ProxyFactory(typeof(T), new RemoteCallInterceptor(_networkConnection.OperationDispatcher, _networkConnection, typeof(T).Name, executionMode));
			return (T)factory.GetProxy();
		}
	}
}