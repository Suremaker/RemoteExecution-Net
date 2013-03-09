using System;

namespace RemoteExecution.IT.Services
{
	public class RemoteService : IRemoteService
	{
		private readonly int _connectionId;
		private readonly IClientService _clientService;

		public RemoteService(int connectionId, IClientService clientService)
		{
			_connectionId = connectionId;
			_clientService = clientService;
		}

		public int GetConnectionId()
		{
			return _connectionId;
		}

		public int ExecuteChainedMethod()
		{
			return _clientService.GetClientValue() * 2;
		}

		public string Hello()
		{
			return "world";
		}
	}
}