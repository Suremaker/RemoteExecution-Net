namespace RemoteExecution.Core.Connections
{
	public interface IClientConnection : IRemoteConnection
	{
		void Open();
	}
}