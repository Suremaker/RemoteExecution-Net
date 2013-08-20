namespace RemoteExecution.Endpoints
{
	public class ServerEndpointConfig
	{
		public static readonly int DefaultMaxConnections = 256;

		public string ApplicationId { get; set; }
		public int MaxConnections { get; set; }
		public ushort Port { get; set; }

		public ServerEndpointConfig(string applicationId, ushort port)
		{
			MaxConnections = DefaultMaxConnections;
			ApplicationId = applicationId;
			Port = port;
		}
	}
}