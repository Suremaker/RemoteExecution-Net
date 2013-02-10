namespace RemoteExecution.Endpoints
{
    public interface IMessage
    {
        string CorrelationId { get; }
        string GroupId { get; }
    }
}