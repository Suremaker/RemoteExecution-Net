using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Endpoints;
using RemoteExecution.Messages;

namespace RemoteExecution.Handling
{
    internal class RequestHandler : IHandler
    {
        private readonly object _handler;
        private readonly Type _type;

        public RequestHandler(string id, object handler)
        {
            Id = id;
            _handler = handler;
            _type = handler.GetType();
        }

        #region IHandler Members

        public string Id { get; private set; }

        public void Handle(IMessage msg, IWriteEndpoint writeEndpoint)
        {
            writeEndpoint.Send(new Response(msg.CorrelationId, Execute((Request)msg)));
        }

        #endregion

        private object Execute(Request request)
        {
            try
            {
                return _type.InvokeMember(request.OperationName,
                                          BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null,
                                          _handler, request.Args);
            }
            catch (MissingMemberException)
            {
                return new InvalidOperationException(string.Format(
                    "Unable to call {0}({1}) method on {2} handler: no matching method was found.",
                    request.OperationName,
                    string.Join(",", request.Args.Select(a => a == null ? "null" : a.GetType().Name)),
                    Id));
            }
            catch (TargetInvocationException e)
            {
                return e.InnerException;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}