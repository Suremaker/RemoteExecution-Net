using RemoteExecution.Dispatching;

namespace RemoteExecution.Endpoints
{
	public interface IConfigurableNetworkConnection : INetworkConnection
	{
		new IOperationDispatcher OperationDispatcher { set; get; }
	}
}