using RemoteExecution.Core.Dispatchers;

namespace RemoteExecution.Core.Serializers
{
	public interface IMessageSerializer
	{
		IMessage Deserialize(byte[] msg);
		byte[] Serialize(IMessage msg);
	}
}