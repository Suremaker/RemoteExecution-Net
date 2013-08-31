using System;
using System.Threading;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Connections;
using RemoteExecution.Executors;

namespace RemoteExecution.AT.Helpers.Services
{
	class RemoteService : IRemoteService
	{
		private readonly IBroadcastService _broadcastService;
		private readonly IClientService _clientService;
		private readonly IRemoteConnection _connection;

		public RemoteService(IRemoteConnection connection, IBroadcastRemoteExecutor broadcastRemoteExecutor)
		{
			_connection = connection;
			_clientService = _connection.RemoteExecutor.Create<IClientService>();
			_broadcastService = broadcastRemoteExecutor.Create<IBroadcastService>();
		}

		#region IRemoteService Members

		public void Sleep(TimeSpan time)
		{
			Thread.Sleep(time);
		}

		public void CloseConnectionOnServerSide()
		{
			_connection.Dispose();
		}

		public string GetHexValueUsingCallback(int value)
		{
			return "0x" + _clientService.GetHexValue(value);
		}

		public void NotifyAll(int value)
		{
			_broadcastService.SetValue(value);
		}

		#endregion
	}
}