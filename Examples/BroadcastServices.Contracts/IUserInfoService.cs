using System.Collections.Generic;

namespace BroadcastServices.Contracts
{
	public interface IUserInfoService
	{
		IEnumerable<string> GetRegisteredUsers();
	}
}