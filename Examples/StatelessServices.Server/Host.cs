using RemoteExecution.Dispatching;
using RemoteExecution.Endpoints;
using StatelessServices.Contracts;

namespace StatelessServices.Server
{
	class Host : ServerEndpoint
	{
		private readonly IOperationDispatcher _dispatcher;

		public Host(int maxConnections, ushort port)
			: base(Protocol.Id, maxConnections, port)
		{
			_dispatcher = new OperationDispatcher();
			_dispatcher.RegisterRequestHandler<ICalculator>(new CalculatorService());
			_dispatcher.RegisterRequestHandler<IGreeter>(new GreeterService());
		}

		protected override bool HandleNewConnection(IConfigurableNetworkConnection connection)
		{
			connection.OperationDispatcher = _dispatcher;
			return true;
		}
	}
}