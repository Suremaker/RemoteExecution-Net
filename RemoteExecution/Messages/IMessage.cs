namespace RemoteExecution.Messages
{
    public interface IMessage
    {
        string CorrelationId { get; }
        string GroupId { get; }
    }
}