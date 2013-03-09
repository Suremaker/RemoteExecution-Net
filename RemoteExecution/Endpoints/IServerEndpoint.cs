namespace RemoteExecution.Endpoints
{
	public interface IServerEndpoint : INetworkEndpoint
	{
		void StartListening();
	}
}