using System;
using System.Threading;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Core.Connections;
using RemoteExecution.Core.Executors;

namespace RemoteExecution.AT.Helpers.Services
{
	class RemoteService : IRemoteService
	{
		private readonly IRemoteConnection _connection;
		private readonly IBroadcastService _broadcastService;
		private readonly IClientService _clientService;

		public RemoteService(IRemoteConnection connection, IBroadcastRemoteExecutor broadcastRemoteExecutor)
		{
			_connection = connection;
			_clientService = _connection.Executor.Create<IClientService>();
			_broadcastService = broadcastRemoteExecutor.Create<IBroadcastService>();
		}

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
	}
}