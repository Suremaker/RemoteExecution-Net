using BroadcastServices.Contracts;

namespace BroadcastServices.Server
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly IBroadcastService _broadcastService;
		private readonly ClientContext _clientContext;

		public RegistrationService(ClientContext clientContext, IBroadcastService broadcastService)
		{
			_clientContext = clientContext;
			_broadcastService = broadcastService;
		}

		#region IRegistrationService Members

		public void Register(string name)
		{
			_clientContext.Name = name;
			_broadcastService.UserRegistered(name);
		}

		public string GetUserName()
		{
			return _clientContext.Name;
		}

		#endregion
	}
}