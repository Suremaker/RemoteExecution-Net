namespace RemoteExecution.Endpoints
{
	public class ServerEndpointConfig
	{
		public static readonly int DefaultMaxConnections = 256;

		public ServerEndpointConfig(string applicationId, ushort port)
		{
			MaxConnections = DefaultMaxConnections;
			ApplicationId = applicationId;
			Port = port;
		}

		public string ApplicationId { get; set; }
		public int MaxConnections { get; set; }
		public ushort Port { get; set; }
	}
}