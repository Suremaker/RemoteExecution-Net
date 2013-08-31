using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	public static class Configurator
	{
		public static void Configure()
		{
			TransportLayerResolver.Register(new LidgrenProvider());
		}
	}
}
