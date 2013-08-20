using System;

namespace RemoteExecution.IT.Services
{
	public interface IClientService
	{
		void Callback(TimeSpan timeSpan);
		int GetClientValue();
	}
}