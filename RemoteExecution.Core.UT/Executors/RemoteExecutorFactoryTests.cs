﻿using NUnit.Framework;
using RemoteExecution.Channels;
using RemoteExecution.Dispatchers;
using RemoteExecution.Executors;
using Rhino.Mocks;

namespace RemoteExecution.Core.UT.Executors
{
	[TestFixture]
	public class RemoteExecutorFactoryTests
	{
		private IRemoteExecutorFactory _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new RemoteExecutorFactory();
		}

		#endregion

		[Test]
		public void Should_create_broadcast_remote_executor()
		{
			var broadcastChannel = MockRepository.GenerateMock<IBroadcastChannel>();
			Assert.That(_subject.CreateBroadcastRemoteExecutor(broadcastChannel), Is.InstanceOf<BroadcastRemoteExecutor>());
		}

		[Test]
		public void Should_create_remote_executor()
		{
			var duplexChannel = MockRepository.GenerateMock<IDuplexChannel>();
			var messageDispatcher = MockRepository.GenerateMock<IMessageDispatcher>();
			Assert.That(_subject.CreateRemoteExecutor(duplexChannel, messageDispatcher), Is.InstanceOf<RemoteExecutor>());
		}
	}
}
