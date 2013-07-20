using RemoteExecution.Dispatchers;

namespace RemoteExecution.Endpoints
{
	public class StatelessServerEndpoint : ServerEndpoint
	{
		private readonly IOperationDispatcher _dispatcher;

		public StatelessServerEndpoint(ServerEndpointConfig config, IOperationDispatcher dispatcher)
			: base(config)
		{
			_dispatcher = dispatcher;
		}

		protected override IOperationDispatcher GetDispatcherForNewConnection()
		{
			return _dispatcher;
		}
	}
}