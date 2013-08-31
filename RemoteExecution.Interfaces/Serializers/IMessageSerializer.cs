using RemoteExecution.Dispatchers.Messages;

namespace RemoteExecution.Serializers
{
	public interface IMessageSerializer
	{
		IMessage Deserialize(byte[] msg);
		byte[] Serialize(IMessage msg);
	}
}