using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution
{
	public class RemoteExecutor : IRemoteExecutor
	{
		private readonly IOperationDispatcher _operationDispatcher;
		private readonly IWriteEndpoint _writeEndpoint;

		public RemoteExecutor(IOperationDispatcher operationDispatcher, IWriteEndpoint writeEndpoint)
		{
			_operationDispatcher = operationDispatcher;
			_writeEndpoint = writeEndpoint;
		}

		public T Create<T>()
		{
			var factory = new ProxyFactory(typeof(T), new RemoteCallInterceptor(_operationDispatcher, _writeEndpoint, typeof(T).Name));
			return (T)factory.GetProxy();
		}
	}
}