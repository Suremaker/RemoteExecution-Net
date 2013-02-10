using ObjectSerialization;
using ObjectSerialization.Types;
using RemoteExecution.Endpoints;
using RemoteExecution.Messages;

namespace RemoteExecution.Serialization
{
    public class MessageSerializer
    {
        private static readonly IObjectSerializer _serializer = new ObjectSerializer();

        static MessageSerializer()
        {
            TypeInfoRepository.RegisterPredefined(typeof(Request));
            TypeInfoRepository.RegisterPredefined(typeof(Response));
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