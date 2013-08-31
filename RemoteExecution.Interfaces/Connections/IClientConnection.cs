namespace RemoteExecution.Connections
{
	public interface IClientConnection : IRemoteConnection
	{
		void Open();
	}
}