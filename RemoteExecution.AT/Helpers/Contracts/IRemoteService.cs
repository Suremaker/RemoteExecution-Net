using System;

namespace RemoteExecution.AT.Helpers.Contracts
{
	public interface IRemoteService
	{
		void Sleep(TimeSpan time);
		void CloseConnectionOnServerSide();
		string GetHexValueUsingCallback(int value);
		void NotifyAll(int value);
	}
}