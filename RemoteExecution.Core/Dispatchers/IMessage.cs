namespace RemoteExecution.Core.Dispatchers
{
	/// <summary>
	/// Base interface for message.
	/// </summary>
	public interface IMessage
	{
		/// <summary>
		/// Message correlation id used for determining relation between request and response messages.
		/// </summary>
		string CorrelationId { get; }
		/// <summary>
		/// Type of message, used for determining handler for this message.
		/// </summary>
		string MessageType { get; }
	}
}