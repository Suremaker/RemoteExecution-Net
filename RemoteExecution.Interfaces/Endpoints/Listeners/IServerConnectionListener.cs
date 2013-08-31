using System;
using RemoteExecution.Channels;

namespace RemoteExecution.Endpoints.Listeners
{
	public interface IServerConnectionListener : IDisposable
	{
		/// <summary>
		/// Fires when channel for new connection is opened.
		/// After all event handlers are finished, channel is treated as fully operational and ready for receiving data.
		/// </summary>
		event Action<IDuplexChannel> OnChannelOpen;

		/// <summary>
		/// Returns broadcast channel allowing to send messages to all clients at once.
		/// </summary>
		IBroadcastChannel BroadcastChannel { get; }

		/// <summary>
		/// Returns true if listener is actively listening for incoming connections.
		/// </summary>
		bool IsListening { get; }

		/// <summary>
		/// Starts listening for incoming connections.
		/// </summary>
		void StartListening();
	}
}