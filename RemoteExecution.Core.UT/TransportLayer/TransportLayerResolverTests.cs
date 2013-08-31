using System;
using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Endpoints.Listeners;
using RemoteExecution.TransportLayer;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.TransportLayer
{
	[TestFixture]
	public class TransportLayerResolverTests
	{
		private ITransportLayerProvider StubProvider(string scheme, IClientChannel clientChannel, IServerConnectionListener connectionListener)
		{
			var provider = MockRepository.GenerateMock<ITransportLayerProvider>();
			provider.Stub(p => p.Scheme).Return(scheme);
			provider.Stub(p => p.CreateClientChannelFor(Arg<Uri>.Is.Anything)).Return(clientChannel);
			provider.Stub(p => p.CreateConnectionListenerFor(Arg<Uri>.Is.Anything)).Return(connectionListener);
			return provider;
		}

		[Test]
		public void Should_create_client_channel_throw_exception_for_unknown_schema()
		{
			var ex = Assert.Throws<UnknownTransportLayerException>(() => TransportLayerResolver.CreateClientChannelFor(new Uri("s1://localhost")));
			Assert.That(ex.Message, Is.EqualTo("Unable to resolve transport layer for 's1' scheme."));
		}

		[Test]
		public void Should_create_connection_listener_throw_exception_for_unknown_schema()
		{
			var ex = Assert.Throws<UnknownTransportLayerException>(() => TransportLayerResolver.CreateConnectionListenerFor(new Uri("s1://localhost")));
			Assert.That(ex.Message, Is.EqualTo("Unable to resolve transport layer for 's1' scheme."));
		}

		[Test]
		public void Should_register_throw_if_there_is_provider_for_given_schema()
		{
			TransportLayerResolver.Register(StubProvider("s1", null, null));

			var ex = Assert.Throws<ArgumentException>(() => TransportLayerResolver.Register(StubProvider("s1", null, null)));
			Assert.That(ex.Message, Is.EqualTo("There is already registered provider for 's1' scheme."));
		}

		[Test]
		public void Should_resolve_providers()
		{
			var clientChannel1 = MockRepository.GenerateMock<IClientChannel>();
			var clientChannel2 = MockRepository.GenerateMock<IClientChannel>();
			var connectionListener1 = MockRepository.GenerateMock<IServerConnectionListener>();
			var connectionListener2 = MockRepository.GenerateMock<IServerConnectionListener>();

			var provider1 = StubProvider("scheme1", clientChannel1, connectionListener1);
			var provider2 = StubProvider("scheme2", clientChannel2, connectionListener2);

			TransportLayerResolver.Register(provider1);
			TransportLayerResolver.Register(provider2);

			Assert.That(TransportLayerResolver.CreateClientChannelFor(new Uri("scheme1://localhost")), Is.SameAs(clientChannel1));
			Assert.That(TransportLayerResolver.CreateClientChannelFor(new Uri("scheme2://localhost")), Is.SameAs(clientChannel2));
			Assert.That(TransportLayerResolver.CreateConnectionListenerFor(new Uri("scheme1://localhost")), Is.SameAs(connectionListener1));
			Assert.That(TransportLayerResolver.CreateConnectionListenerFor(new Uri("scheme2://localhost")), Is.SameAs(connectionListener2));
		}
	}
}
