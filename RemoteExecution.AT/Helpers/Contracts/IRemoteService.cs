﻿using System;

namespace RemoteExecution.AT.Helpers.Contracts
{
	public interface IRemoteService
	{
		void Sleep(TimeSpan time);
	}
}