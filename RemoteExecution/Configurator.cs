using RemoteExecution.Core.TransportLayer;
using RemoteExecution.TransportLayer.Lidgren;

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
