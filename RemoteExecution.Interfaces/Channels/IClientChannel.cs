namespace RemoteExecution.Channels
{
	/// <summary>
	/// Client channel allowing to send and receive messages. 
	/// Client channel has to be opened with Open() method before use
	/// and can be reopened after it has been closed.
	/// </summary>
	public interface IClientChannel : IDuplexChannel
	{
		/// <summary>
		/// Opens channel for sending and receiving messages.
		/// If channel has been closed, this method reopens it.
		/// </summary>
		void Open();
	}
}