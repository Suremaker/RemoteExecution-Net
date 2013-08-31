using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RemoteExecution.Connections;

namespace StatefulServices.Server
{
	internal class SharedContext
	{
		private readonly IDictionary<IRemoteConnection, ClientContext> _clients = new ConcurrentDictionary<IRemoteConnection, ClientContext>();
		public void AddClient(IRemoteConnection connection, ClientContext clientContext)
		{
			_clients.Add(connection, clientContext);
		}

		public IEnumerable<string> GetRegisteredClients()
		{
			return _clients.Values.Where(v => v.IsRegistered).Select(v => v.Name).ToArray();
		}

		public void RemoveClient(IRemoteConnection connection)
		{
			_clients.Remove(connection);
		}
	}
}