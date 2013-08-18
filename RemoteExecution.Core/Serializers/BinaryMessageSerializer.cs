using System.Reflection;
using ObjectSerialization;
using ObjectSerialization.Types;
using RemoteExecution.Core.Dispatchers;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Serializers
{
	public class BinaryMessageSerializer : IMessageSerializer
	{
		private static readonly IObjectSerializer _serializer = new ObjectSerializer();

		static BinaryMessageSerializer()
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(typeof(RequestMessage).Assembly);
		}

		public static void RegsterSerializableFrom(Assembly assembly)
		{
			TypeInfoRepository.RegisterPredefinedUsingSerializableFrom(assembly);
		}

		public IMessage Deserialize(byte[] msg)
		{
			return _serializer.Deserialize<IMessage>(msg);
		}

		public byte[] Serialize(IMessage msg)
		{
			return _serializer.Serialize(msg);
		}
	}
}