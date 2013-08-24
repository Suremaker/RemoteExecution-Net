using System;
using NUnit.Framework;

namespace RemoteExecution.TransportLayer.Lidgren.IT
{
	[TestFixture]
	public class LidgrenProviderTests
	{
		private LidgrenProvider _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new LidgrenProvider();
		}

		[Test]
		public void Should_create_client_throw_if_scheme_is_wrong()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateClientChannelFor(new Uri("other://localhost:3221/appId")));
			Assert.That(ex.Message, Is.EqualTo("Invalid scheme."));
		}

		[Test]
		public void Should_create_connection_listener_throw_if_scheme_is_wrong()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateConnectionListenerFor(new Uri("other://0.0.0.0:3221/appId")));
			Assert.That(ex.Message, Is.EqualTo("Invalid scheme."));
		}

		[Test]
		public void Should_create_client_throw_if_no_app_id_is_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateClientChannelFor(new Uri("net://localhost:3221")));
			Assert.That(ex.Message, Is.EqualTo("No application id provided."));
		}

		[Test]
		public void Should_create_connection_listener_throw_if_no_app_id_is_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateConnectionListenerFor(new Uri("net://0.0.0.0:3221")));
			Assert.That(ex.Message, Is.EqualTo("No application id provided."));
		}

		[Test]
		public void Should_create_client_throw_if_wrong_app_id_is_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateClientChannelFor(new Uri("net://localhost:3221/appId/appId2")));
			Assert.That(ex.Message, Is.EqualTo("Application id cannot contain '/' character."));
		}

		[Test]
		public void Should_create_connection_listener_throw_if_wrong_app_id_is_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateConnectionListenerFor(new Uri("net://0.0.0.0:3221/appId/appId2")));
			Assert.That(ex.Message, Is.EqualTo("Application id cannot contain '/' character."));
		}

		[Test]
		public void Should_create_client_throw_if_no_port_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateClientChannelFor(new Uri("net://localhost/appId")));
			Assert.That(ex.Message, Is.EqualTo("No port provided."));
		}

		[Test]
		public void Should_create_connection_listener_throw_if_no_port_provided()
		{
			var ex = Assert.Throws<ArgumentException>(() => _subject.CreateConnectionListenerFor(new Uri("net://0.0.0.0/appId")));
			Assert.That(ex.Message, Is.EqualTo("No port provided."));
		}

		[Test]
		public void Should_create_client()
		{
			var client = _subject.CreateClientChannelFor(new Uri("net://localhost:3243/appId"));
			Assert.That(client, Is.Not.Null);
		}

		[Test]
		public void Should_create_connection_listener_throw_if_host_is_not_ip()
		{
			var ex = Assert.Throws<FormatException>(() => _subject.CreateConnectionListenerFor(new Uri("net://localhost:3232/appId")));
			Assert.That(ex.Message, Is.EqualTo("An invalid IP address was specified."));
		}

		[Test]
		public void Should_create_connection_listener()
		{
			var client = _subject.CreateConnectionListenerFor(new Uri("net://0.0.0.0:3243/appId"));
			Assert.That(client, Is.Not.Null);
		}
	}
}