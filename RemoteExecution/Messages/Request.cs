using RemoteExecution.Endpoints;

namespace RemoteExecution.Messages
{
    public class Request : IMessage
    {
        private static readonly object[] EMPTY = new object[0];

        public object[] Args { get; set; }
        public string OperationName { get; set; }

        public Request() { }

        public Request(string id, string interfaceName, string operationName, object[] args)
        {
            CorrelationId = id;
            GroupId = interfaceName;
            OperationName = operationName;
            Args = args ?? EMPTY;
        }

        #region IMessage Members

        public string CorrelationId { get; set; }
        public string GroupId { get; set; }

        #endregion
    }
}