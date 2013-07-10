using System;
using System.Collections.Generic;
using StatefulServices.Contracts;

namespace StatefulServices.Server
{
	internal class UserInfoService : IUserInfoService
	{
		private readonly SharedContext _sharedContext;
		private readonly UserContext _userContext;

		public UserInfoService(SharedContext sharedContext, UserContext userContext)
		{
			_sharedContext = sharedContext;
			_userContext = userContext;
		}

		public IEnumerable<string> GetRegisteredUsers()
		{
			if(!_userContext.IsRegistered)
				throw new UnauthorizedAccessException("User is not registered");
			return _sharedContext.GetRegisteredClients();
		}
	}
}