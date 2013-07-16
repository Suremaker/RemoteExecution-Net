using System;
using NUnit.Framework;
using RemoteExecution.Messages;

namespace RemoteExecution.UT.Messages
{
	[TestFixture]
	public class ExceptionResponseTests
	{
		[Test]
		public void Should_set_assembly_qualified_name_as_exception_identifier()
		{
			var subject = new ExceptionResponse("id", typeof(InvalidOperationException), "test");
			Assert.That(subject.ExceptionType, Is.EqualTo(typeof(InvalidOperationException).AssemblyQualifiedName));
		}

		[Test]
		[TestCase(typeof(InvalidOperationException))]
		[TestCase(typeof(Exception))]
		[TestCase(typeof(ArgumentException))]
		[TestCase(typeof(ArgumentNullException))]
		[TestCase(typeof(NullReferenceException))]
		[TestCase(typeof(ArgumentOutOfRangeException))]
		public void Should_reinstantiate_exception_and_throw(Type exceptionType)
		{
			var subject = new ExceptionResponse("id", exceptionType, "test");

			try
			{
				var x = subject.Value;
				Assert.Fail("Expected exception");
			}
			catch (Exception ex)
			{
				Assert.That(ex, Is.InstanceOf(exceptionType));
				Assert.That(ex.Message, Is.StringContaining(subject.Message));
			}
		}
	}
}