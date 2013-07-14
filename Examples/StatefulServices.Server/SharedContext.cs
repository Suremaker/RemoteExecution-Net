using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RemoteExecution.Endpoints;

namespace StatefulServices.Server
{
	internal class SharedContext
	{
		private readonly IDictionary<INetworkConnection, ClientContext> _clients = new ConcurrentDictionary<INetworkConnection, ClientContext>();
		public void AddClient(INetworkConnection connection, ClientContext clientContext)
		{
			_clients.Add(connection, clientContext);
		}

		public void RemoveClient(INetworkConnection connection)
		{
			_clients.Remove(connection);
		}

		public IEnumerable<string> GetRegisteredClients()
		{
			return _clients.Values.Where(v => v.IsRegistered).Select(v => v.Name).ToArray();
		}
	}
}