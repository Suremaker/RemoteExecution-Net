using RemoteExecution.TransportLayer;

namespace RemoteExecution
{
	/// <summary>
	/// Remote execution framework configurator.
	/// </summary>
	public static class Configurator
	{
		/// <summary>
		/// Configures remote execution framework.
		/// Registers all supported transport layer providers.
		/// </summary>
		public static void Configure()
		{
			TransportLayerResolver.Register(new LidgrenProvider());
		}
	}
}
