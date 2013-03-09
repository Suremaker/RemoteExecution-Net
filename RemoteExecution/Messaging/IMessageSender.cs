namespace RemoteExecution.Messaging
{
    public interface IMessageSender
    {
        void Send(IMessage message);
    }
}