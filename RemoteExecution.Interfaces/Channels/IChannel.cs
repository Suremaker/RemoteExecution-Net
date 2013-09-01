using System;

namespace RemoteExecution.Channels
{
	/// <summary>
	/// Channel interface offering common channel features.
	/// This channel implements IDisposable interface, where Dispose() method closes it.
	/// </summary>
	public interface IChannel : IDisposable
	{
		/// <summary>
		/// Event fired when channel is closed.
		/// </summary>
		event Action Closed;
		/// <summary>
		/// Unique identifier of channel.
		/// </summary>
		Guid Id { get; }
		/// <summary>
		/// Returns true if channel is opened, otherwise false.
		/// </summary>
		bool IsOpen { get; }
	}
}