using System;
using System.Linq;
using System.Reflection;
using RemoteExecution.Channels;
using RemoteExecution.Messages;

namespace RemoteExecution.Handlers
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
			var request = (IRequest)msg;

			if (request.IsResponseExpected)
				ExecuteWithResponse(request, messageChannel);
			else
				ExecuteWithoutResponse(request);
		}

		#endregion

		private object Execute(IRequest request)
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

		private void ExecuteWithResponse(IRequest msg, IOutgoingMessageChannel messageChannel)
		{
			try
			{
				messageChannel.Send(new Response(msg.CorrelationId, Execute(msg)));
			}
			catch (Exception e)
			{
				messageChannel.Send(new ExceptionResponse(msg.CorrelationId, e.GetType(), e.Message));
			}
		}

		private void ExecuteWithoutResponse(IRequest msg)
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