using System;
using System.Linq;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers.Messages;
using RemoteExecution.Serializers;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Serializers
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
	public class BinaryMessageSerializerTests
	{
		private IMessageSerializer _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new BinaryMessageSerializer();
		}

		#endregion

		[Test]
		public void ShouldRegisterSerializableTypesFromAssembly()
		{
			BinaryMessageSerializer.RegsterSerializableFrom(GetType().Assembly);
			var bytes1 = _subject.Serialize(new ResponseMessage("1", new Greeting1 { Hello = "hello" }));
			var bytes2 = _subject.Serialize(new ResponseMessage("1", new Greeting2 { Hello = "hello" }));
			Assert.That(bytes1.Length, Is.LessThan(bytes2.Length));
		}

		[Test]
		public void ShouldSerializeRequest()
		{
			var expected = new RequestMessage("1", "interface", "operation", new object[] { 1, 2, "string" }, true) { Channel = MockRepository.GenerateMock<IOutputChannel>() };

			var actual = _subject.Deserialize(_subject.Serialize(expected));
			Assert.That(actual, Is.TypeOf<RequestMessage>());
			Assert.That(actual.MessageType, Is.EqualTo(expected.MessageType));
			Assert.That(actual.CorrelationId, Is.EqualTo(expected.CorrelationId));

			var actualRequest = (RequestMessage)actual;
			Assert.That(actualRequest.Args.SequenceEqual(expected.Args));
			Assert.That(actualRequest.MethodName, Is.EqualTo(expected.MethodName));
			Assert.That(actualRequest.IsResponseExpected, Is.EqualTo(expected.IsResponseExpected));
			Assert.That(actualRequest.Channel, Is.Null);
		}

		[Test]
		public void ShouldSerializeResponse()
		{
			var expected = new ResponseMessage("1", Guid.NewGuid());
			var actual = _subject.Deserialize(_subject.Serialize(expected));
			Assert.That(actual, Is.TypeOf<ResponseMessage>());
			Assert.That(actual.MessageType, Is.EqualTo(expected.MessageType));
			Assert.That(actual.CorrelationId, Is.EqualTo(expected.CorrelationId));

			var actualResponse = (ResponseMessage)actual;
			Assert.That(actualResponse.Value, Is.EqualTo(expected.Value));
		}
	}
}