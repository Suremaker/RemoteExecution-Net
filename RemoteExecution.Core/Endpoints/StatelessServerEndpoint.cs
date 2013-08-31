﻿using RemoteExecution.Core.Config;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Endpoints.Listeners;

namespace RemoteExecution.Core.Endpoints
{
	public class StatelessServerEndpoint : ServerEndpoint
	{
		private readonly IOperationDispatcher _dispatcher;

		public StatelessServerEndpoint(IServerConnectionListener listener, IOperationDispatcher dispatcher)
			: this(listener, dispatcher, new ServerConfig()) { }

		public StatelessServerEndpoint(string listenerUri, IOperationDispatcher dispatcher)
			: this(listenerUri, dispatcher, new ServerConfig()) { }

		public StatelessServerEndpoint(IServerConnectionListener listener, IOperationDispatcher dispatcher, IServerConfig config)
			: base(listener, config)
		{
			_dispatcher = dispatcher;
		}

		public StatelessServerEndpoint(string listenerUri, IOperationDispatcher dispatcher, IServerConfig config)
			: base(listenerUri, config)
		{
			_dispatcher = dispatcher;
		}

		protected override IOperationDispatcher GetOperationDispatcher()
		{
			return _dispatcher;
		}
	}
}