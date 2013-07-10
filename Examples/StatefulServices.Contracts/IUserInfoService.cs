using System.Collections.Generic;

namespace StatefulServices.Contracts
{
	public interface IUserInfoService
	{
		IEnumerable<string> GetRegisteredUsers();
	}
}