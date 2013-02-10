using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Remoting;
using Spring.Aop.Framework;

namespace RemoteExecution
{
    public static class RemoteExecutor
    {
        public static T Create<T>(IOperationDispatcher operationDispatcher, IWriteEndpoint writeEndpoint)
        {
            var factory = new ProxyFactory(typeof(T), new RemoteCallInterceptor(operationDispatcher, writeEndpoint, typeof(T).Name));
            return (T)factory.GetProxy();
        }
    }
}