using System;
using NUnit.Framework;
using RemoteExecution.Core.Dispatchers.Messages;

namespace RemoteExecution.Core.UT.Dispatchers.Messages
{
	[TestFixture]
	public class ExceptionResponseMessageTests
	{
		[Test]
		[TestCase(typeof(InvalidOperationException))]
		[TestCase(typeof(Exception))]
		[TestCase(typeof(ArgumentException))]
		[TestCase(typeof(ArgumentNullException))]
		[TestCase(typeof(NullReferenceException))]
		[TestCase(typeof(ArgumentOutOfRangeException))]
		public void Should_reinstantiate_exception_and_throw(Type exceptionType)
		{
			var subject = new ExceptionResponseMessage("id", exceptionType, "test");

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

		[Test]
		public void Should_set_assembly_qualified_name_as_exception_identifier()
		{
			var subject = new ExceptionResponseMessage("id", typeof(InvalidOperationException), "test");
			Assert.That(subject.ExceptionType, Is.EqualTo(typeof(InvalidOperationException).AssemblyQualifiedName));
		}
	}
}