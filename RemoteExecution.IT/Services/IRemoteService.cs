using System;

namespace RemoteExecution.IT.Services
{
	public interface IRemoteService
	{
		void Broadcast(int number);
		void CloseConnectionOnServerSide();
		int ExecuteChainedMethod();
		int GetConnectionId();
		string Hello();
		void Sleep(TimeSpan timeSpan);
		void ThrowException();
	}
}