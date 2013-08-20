using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.Dispatchers.Handlers
{
	internal class RequestHandler : IMessageHandler
	{
		public object Handler { get; private set; }
		public Type InterfaceType { get; private set; }

		public RequestHandler(Type interfaceType, object handler)
		{
			InterfaceType = interfaceType;
			Handler = handler;
			HandledMessageType = interfaceType.Name;
			HandlerGroupId = interfaceType.GUID;
		}

		#region IMessageHandler Members

		public string HandledMessageType { get; private set; }
		public Guid HandlerGroupId { get; private set; }
		public void Handle(IMessage message)
		{
			var request = (RequestMessage)message;

			if (request.IsResponseExpected)
				ExecuteWithResponse(request);
			else
				ExecuteWithoutResponse(request);
		}

		#endregion

		private object Execute(RequestMessage requestMessage)
		{
			try
			{
				return InterfaceType.InvokeMember(requestMessage.MethodName, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, Handler, requestMessage.Args);
			}
			catch (MissingMemberException)
			{
				throw new InvalidOperationException(string.Format(
					"Unable to call {0}({1}) method on {2} handler: no matching method was found.",
					requestMessage.MethodName,
					string.Join(",", requestMessage.Args.Select(a => a == null ? "null" : a.GetType().Name)),
					HandledMessageType));
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		private void ExecuteWithResponse(RequestMessage msg)
		{
			try
			{
				msg.Channel.Send(new ResponseMessage(msg.CorrelationId, Execute(msg)));
			}
			catch (Exception e)
			{
				msg.Channel.Send(new ExceptionResponseMessage(msg.CorrelationId, e.GetType(), e.Message));
			}
		}

		private void ExecuteWithoutResponse(RequestMessage msg)
		{
			try
			{
				Execute(msg);
			}
			catch (Exception)
			{

			}
		}
	}
}