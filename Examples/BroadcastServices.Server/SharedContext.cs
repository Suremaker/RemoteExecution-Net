using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RemoteExecution.Endpoints;

namespace BroadcastServices.Server
{
	internal class SharedContext
	{
		private readonly IDictionary<INetworkConnection, UserContext> _clients = new ConcurrentDictionary<INetworkConnection, UserContext>();
		public void AddClient(INetworkConnection connection, UserContext userContext)
		{
			_clients.Add(connection, userContext);
		}

		public void RemoveClient(INetworkConnection connection)
		{
			_clients.Remove(connection);
		}

		public UserContext GetUser(INetworkConnection connection)
		{
			return _clients[connection];
		}

		public IEnumerable<string> GetRegisteredClients()
		{
			return _clients.Values.Where(v => v.IsRegistered).Select(v => v.Name).ToArray();
		}
	}
}