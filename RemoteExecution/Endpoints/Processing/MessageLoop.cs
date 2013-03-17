using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;

namespace RemoteExecution.Endpoints.Processing
{
	internal class MessageLoop
	{
		private readonly NetPeer _peer;
		private readonly Action<NetIncomingMessage> _handleMessage;
		private readonly Thread _thread;
		private readonly SemaphoreSlim _semaphore;

		public MessageLoop(NetPeer peer, Action<NetIncomingMessage> handleMessage)
		{
			_peer = peer;
			_handleMessage = handleMessage;
			_thread = new Thread(Run) { Name = "Message loop", IsBackground = true };

			_semaphore = new SemaphoreSlim(0);
			_thread.Start();
		}

		private void Run()
		{
			SetSynchronizationContext();
			_peer.RegisterReceivedCallback(MessageReady);
			_semaphore.Wait();
			_peer.UnregisterReceivedCallback(MessageReady);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void SetSynchronizationContext()
		{
			if (SynchronizationContext.Current != null)
				return;
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
		}

		private void MessageReady(object obj)
		{
			var msg = _peer.ReadMessage();
			if (msg != null)
				Task.Factory.StartNew(() => _handleMessage(msg));
		}

		public void Dispose()
		{
			_semaphore.Release();
			_thread.Join();
			_semaphore.Dispose();
		}
	}
}