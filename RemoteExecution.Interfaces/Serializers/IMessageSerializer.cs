using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Serializers
{
	/// <summary>
	/// Message serializer interface.
	/// </summary>
	public interface IMessageSerializer
	{
		/// <summary>
		/// Deserializes byte array into message object.
		/// </summary>
		/// <param name="msg">Serialized message.</param>
		/// <returns>Deserialized message.</returns>
		IMessage Deserialize(byte[] msg);

		/// <summary>
		/// Serializes message into byte array.
		/// </summary>
		/// <param name="msg">Message to serialize.</param>
		/// <returns>Serialized message.</returns>
		byte[] Serialize(IMessage msg);
	}
}