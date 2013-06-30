using System;

namespace RemoteExecution.IT.Services
{
	public interface IClientService
	{
		int GetClientValue();
		void Callback(TimeSpan timeSpan);
	}
}