using System;

namespace RemoteExecution.AT.Helpers.Contracts
{
	public interface IRemoteService
	{
		void CloseConnectionOnServerSide();
		string GetHexValueUsingCallback(int value);
		void NotifyAll(int value);
		void Sleep(TimeSpan time);
	}
}