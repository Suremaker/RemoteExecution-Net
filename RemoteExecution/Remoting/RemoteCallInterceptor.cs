using AopAlliance.Intercept;
using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using RemoteExecution.Handling;
using RemoteExecution.Messages;

namespace RemoteExecution.Remoting
{
    internal class RemoteCallInterceptor : IMethodInterceptor
    {
        private readonly IWriteEndpoint _endpoint;
        private readonly string _interfaceName;
        private readonly IOperationDispatcher _operationDispatcher;

        public RemoteCallInterceptor(IOperationDispatcher operationDispatcher, IWriteEndpoint endpoint, string interfaceName)
        {
            _operationDispatcher = operationDispatcher;
            _endpoint = endpoint;
            _interfaceName = interfaceName;
        }

        #region IMethodInterceptor Members

        public object Invoke(IMethodInvocation invocation)
        {
            var handler = new ResponseHandler();

            _operationDispatcher.AddHandler(handler);
            try
            {
                _endpoint.Send(new Request(handler.Id, _interfaceName, invocation.Method.Name, invocation.Arguments));
                handler.Wait();
            }
            finally
            {
                _operationDispatcher.RemoveHandler(handler);
            }
            return handler.GetValue();
        }

        #endregion
    }
}