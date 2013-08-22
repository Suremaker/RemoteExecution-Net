using System;
using System.Threading;
using RemoteExecution.AT.Helpers.Contracts;
using RemoteExecution.Core.Connections;

namespace RemoteExecution.AT.Helpers.Services
{
	class RemoteService : IRemoteService
	{
		private readonly IRemoteConnection _connection;

		public RemoteService(IRemoteConnection connection)
		{
			_connection = connection;
		}

		public void Sleep(TimeSpan time)
		{
			Thread.Sleep(time);
		}

		public void CloseConnectionOnServerSide()
		{
			_connection.Dispose();
		}
	}
}