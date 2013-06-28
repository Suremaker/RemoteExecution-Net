using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Messaging;

namespace RemoteExecution.Handling
{
	internal class RequestHandler : IHandler
	{
		private readonly object _handler;
		private readonly Type _type;

		public RequestHandler(Type interfaceType, object handler)
		{
			Id = interfaceType.Name;
			_handler = handler;
			_type = interfaceType;
		}

		#region IHandler Members

		public string Id { get; private set; }

		public void Handle(IMessage msg, IMessageChannel messageChannel)
		{
			if (msg.CorrelationId != null)
				ExecuteWithResponse(msg, messageChannel);
			else
				ExecuteWithoutResponse(msg);
		}

		private void ExecuteWithoutResponse(IMessage msg)
		{
			try
			{
				Execute((Request)msg);
			}
			catch (Exception)
			{

			}
		}

		private void ExecuteWithResponse(IMessage msg, IMessageChannel messageChannel)
		{
			try
			{
				messageChannel.Send(new Response(msg.CorrelationId, Execute((Request)msg)));
			}
			catch (Exception e)
			{
				messageChannel.Send(new ExceptionResponse(msg.CorrelationId, e.GetType(), e.Message));
			}
		}

		#endregion

		private object Execute(Request request)
		{
			try
			{
				return _type.InvokeMember(request.OperationName, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, _handler, request.Args);
			}
			catch (MissingMemberException)
			{
				throw new InvalidOperationException(string.Format(
					"Unable to call {0}({1}) method on {2} handler: no matching method was found.",
					request.OperationName,
					string.Join(",", request.Args.Select(a => a == null ? "null" : a.GetType().Name)),
					Id));
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}
	}
}