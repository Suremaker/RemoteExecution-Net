namespace RemoteExecution.Endpoints.Adapters
{
	public interface IClientEndpointAdapter : IEndpointAdapter
	{
		void ConnectTo(string host, ushort port);
	}
}