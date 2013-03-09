using System;
using System.Collections.Concurrent;
using RemoteExecution.Handling;
using RemoteExecution.Messaging;

namespace RemoteExecution.Dispatching
{
    public class OperationDispatcher : IOperationDispatcher
    {
        private readonly ConcurrentDictionary<string, IHandler> _handlers = new ConcurrentDictionary<string, IHandler>();

        #region IOperationDispatcher Members

        public void RegisterFor<TInterface>(TInterface handler)
        {
            RegisterFor(typeof(TInterface), handler);
        }

        public void AddHandler(IHandler handler)
        {
            _handlers.AddOrUpdate(handler.Id, k => handler, (k, v) => handler);
        }

        public void RemoveHandler(IHandler handler)
        {
            _handlers.TryRemove(handler.Id, out handler);
        }

        public void Unregister<TInterface>()
        {
            IHandler handler;
            _handlers.TryRemove(typeof(TInterface).Name, out handler);
        }

        public void Dispatch(IMessage msg, IMessageSender messageSender)
        {
            IHandler handler;
            if (_handlers.TryGetValue(msg.GroupId, out handler))
                handler.Handle(msg, messageSender);
            else
                HandleUndefinedType(msg, messageSender);
        }

        #endregion

        public void RegisterFor(Type interfaceType, object handler)
        {
            if (!interfaceType.IsInstanceOfType(handler))
                throw new ArgumentException(
                    string.Format(
                        "Unable to register {0} handler: it does not implement {1} interface.",
                        handler.GetType().Name,
                        interfaceType.Name));

            AddHandler(new RequestHandler(interfaceType.Name, handler));
        }

        private void HandleUndefinedType(IMessage msg, IMessageSender messageSender)
        {
            if (!(msg is Request))
                return;
            string message = string.Format("No handler is defined for {0} type.", msg.GroupId);
            messageSender.Send(new Response(msg.CorrelationId, new InvalidOperationException(message)));
        }
    }
}