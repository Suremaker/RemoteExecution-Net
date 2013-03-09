namespace RemoteExecution.Endpoints
{
	public interface IClientEndpoint : INetworkEndpoint
	{
		INetworkConnection ConnectTo(string host, ushort port);		
	}
}