using System;
using System.Collections.Generic;
using BroadcastServices.Contracts;

namespace BroadcastServices.Server
{
	internal class UserInfoService : IUserInfoService
	{
		private readonly ClientContext _clientContext;
		private readonly SharedContext _sharedContext;

		public UserInfoService(SharedContext sharedContext, ClientContext clientContext)
		{
			_sharedContext = sharedContext;
			_clientContext = clientContext;
		}

		#region IUserInfoService Members

		public IEnumerable<string> GetRegisteredUsers()
		{
			if(!_clientContext.IsRegistered)
				throw new UnauthorizedAccessException("User is not registered");
			return _sharedContext.GetRegisteredClients();
		}

		#endregion
	}
}