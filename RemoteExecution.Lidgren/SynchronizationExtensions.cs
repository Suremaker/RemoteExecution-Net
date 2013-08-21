﻿using System;
using System.IO;
using System.Threading;
using Lidgren.Network;

namespace RemoteExecution.Lidgren
{
	public static class SynchronizationExtensions
	{
		private static readonly TimeSpan _synchronizationTimeSpan = TimeSpan.FromMilliseconds(25);

		public static void WaitForConnectionToOpen(this NetConnection connection)
		{
			while (connection.Status != NetConnectionStatus.Connected)
			{
				if (connection.Status == NetConnectionStatus.Disconnected)
					throw new IOException("Connection closed.");

				Thread.Sleep(_synchronizationTimeSpan);
			}
		}

		public static void WaitForClose(this NetPeer netPeer)
		{
			while (netPeer.Status != NetPeerStatus.NotRunning)
				Thread.Sleep(_synchronizationTimeSpan);
		}
	}
}