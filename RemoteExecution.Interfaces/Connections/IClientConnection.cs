namespace RemoteExecution.Connections
{
	/// <summary>
	/// Client connection interface allowing to execute operations remotely 
	/// or configure handlers for operations incoming from remote end.
	/// Client connection has to be opened with Open() method before use 
	/// and can be reopened after it has been closed.
	/// </summary>
	public interface IClientConnection : IRemoteConnection
	{
		/// <summary>
		/// Opens connection and make it ready for sending and handling operation requests.
		/// If connection has been closed, this method reopens it.
		/// </summary>
		void Open();
	}
}