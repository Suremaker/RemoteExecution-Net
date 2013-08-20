using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Messages;
using RemoteExecution.Serialization;

namespace RemoteExecution.UT.Serialization
{
	[Serializable]
	class Greeting1
	{
		public string Hello { get; set; }
	}

	class Greeting2
	{
		public string Hello { get; set; }
	}

	[TestFixture]
	public class MessageSerializerTests
	{
		private MessageSerializer _serializer;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_serializer = new MessageSerializer();
		}

		#endregion

		[Test]
		public void ShouldRegisterSerializableTypesFromAssembly()
		{
			MessageSerializer.RegsterSerializableFrom(GetType().Assembly);
			var bytes1 = _serializer.Serialize(new Response("1", new Greeting1 { Hello = "hello" }));
			var bytes2 = _serializer.Serialize(new Response("1", new Greeting2 { Hello = "hello" }));
			Assert.That(bytes1.Length, Is.LessThan(bytes2.Length));
		}

		[Test]
		public void ShouldSerializeRequest()
		{
			var expected = new Request("1", "interface", "operation", new object[] { 1, 2, "string" }, true);
			var actual = _serializer.Deserialize(_serializer.Serialize(expected));
			Assert.That(actual, Is.TypeOf<Request>());
			Assert.That(actual.GroupId, Is.EqualTo(expected.GroupId));
			Assert.That(actual.CorrelationId, Is.EqualTo(expected.CorrelationId));

			var actualRequest = (Request)actual;
			Assert.That(actualRequest.Args.SequenceEqual(expected.Args));
			Assert.That(actualRequest.OperationName, Is.EqualTo(expected.OperationName));
			Assert.That(actualRequest.IsResponseExpected, Is.EqualTo(expected.IsResponseExpected));
		}

		[Test]
		public void ShouldSerializeResponse()
		{
			var expected = new Response("1", Guid.NewGuid());
			var actual = _serializer.Deserialize(_serializer.Serialize(expected));
			Assert.That(actual, Is.TypeOf<Response>());
			Assert.That(actual.GroupId, Is.EqualTo(expected.GroupId));
			Assert.That(actual.CorrelationId, Is.EqualTo(expected.CorrelationId));

			var actualResponse = (Response)actual;
			Assert.That(actualResponse.Value, Is.EqualTo(expected.Value));
		}
	}
}
