using System.Reflection;
using ObjectSerialization;
using ObjectSerialization.Types;
using RemoteExecution.Messages;

namespace RemoteExecution.Serialization
{
	public class MessageSerializer
	{
		private static readonly IObjectSerializer _serializer = new ObjectSerializer();

		static MessageSerializer()
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(typeof(Request).Assembly);
		}

		public IMessage Deserialize(byte[] msg)
		{
			return _serializer.Deserialize<IMessage>(msg);
		}

		public byte[] Serialize(IMessage msg)
		{
			return _serializer.Serialize(msg);
		}

		public static void RegsterSerializableFrom(Assembly assembly)
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(assembly);
		}
	}
}