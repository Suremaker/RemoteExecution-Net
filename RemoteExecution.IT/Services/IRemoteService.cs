using System;

namespace RemoteExecution.IT.Services
{
	public interface IRemoteService
	{
		int GetConnectionId();
		int ExecuteChainedMethod();
		string Hello();
		void ThrowException();
		void CloseConnectionOnServerSide();
		void Sleep(TimeSpan timeSpan);
		void Broadcast(int number);
	}
}