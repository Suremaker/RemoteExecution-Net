namespace RemoteExecution.Endpoints
{
    public interface IWriteEndpoint
    {
        void Send(IMessage message);
    }
}