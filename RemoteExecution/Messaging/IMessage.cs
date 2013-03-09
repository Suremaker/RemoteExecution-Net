namespace RemoteExecution.Messaging
{
    public interface IMessage
    {
        string CorrelationId { get; }
        string GroupId { get; }
    }
}