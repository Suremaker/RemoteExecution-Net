using System.Threading;

namespace RemoteExecution.Endpoints.Processing
{
	public class MessageLoop : IMessageLoop
	{
		private readonly INetworkEndpoint _endpoint;
		private readonly Thread _thread;
		private bool _shouldStop;

		public MessageLoop(INetworkEndpoint endpoint)
		{
			_endpoint = endpoint;
			_thread = new Thread(Run) { Name = "Message loop" };

			_shouldStop = false;
			_thread.Start();
		}

		private void Run()
		{
			while (!_shouldStop)
			{
				if (!_endpoint.ProcessMessage())
					Thread.Sleep(50);
			}
		}

		public void Dispose()
		{
			_shouldStop = true;
			_thread.Join();
		}
	}
}