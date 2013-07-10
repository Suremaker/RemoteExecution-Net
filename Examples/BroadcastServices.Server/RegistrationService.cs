using BroadcastServices.Contracts;

namespace BroadcastServices.Server
{
	internal class RegistrationService : IRegistrationService
	{
		private readonly UserContext _userContext;
		private readonly IBroadcastService _broadcastService;

		public RegistrationService(UserContext userContext, IBroadcastService broadcastService)
		{
			_userContext = userContext;
			_broadcastService = broadcastService;
		}

		public void Register(string name)
		{
			_userContext.Name = name;
			_broadcastService.UserRegistered(name);
		}

		public string GetUserName()
		{
			return _userContext.Name;
		}
	}
}