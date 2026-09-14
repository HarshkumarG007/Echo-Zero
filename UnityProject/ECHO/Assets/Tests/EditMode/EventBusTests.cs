using System;
using NUnit.Framework;
using EchoZero.Core.Events;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Unit tests for EventBus&lt;T&gt;.
    /// TASK: TASK-002
    /// </summary>
    [TestFixture]
    public class EventBusTests
    {
        private struct TestEvent { public int Value; }

        [TearDown]
        public void TearDown()
        {
            EventBus<TestEvent>.Clear();
        }

        [Test]
        public void Subscribe_ThenPublish_HandlerIsCalled()
        {
            var called = false;
            EventBus<TestEvent>.Subscribe(_ => called = true);
            EventBus<TestEvent>.Publish(new TestEvent());
            Assert.IsTrue(called, "Handler should be called after Publish.");
        }

        [Test]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EventBus<TestEvent>.Publish(new TestEvent()));
        }

        [Test]
        public void Unsubscribe_ThenPublish_HandlerNotCalled()
        {
            var called = false;
            Action<TestEvent> handler = _ => called = true;
            EventBus<TestEvent>.Subscribe(handler);
            EventBus<TestEvent>.Unsubscribe(handler);
            EventBus<TestEvent>.Publish(new TestEvent());
            Assert.IsFalse(called, "Handler must NOT be called after Unsubscribe.");
        }

        [Test]
        public void MultipleSubscribers_AllCalledOnPublish()
        {
            var count = 0;
            EventBus<TestEvent>.Subscribe(_ => count++);
            EventBus<TestEvent>.Subscribe(_ => count++);
            EventBus<TestEvent>.Subscribe(_ => count++);
            EventBus<TestEvent>.Publish(new TestEvent());
            Assert.AreEqual(3, count, "All three subscribers should be called.");
        }

        [Test]
        public void PublishedValue_IsReceivedBySubscriber()
        {
            var received = 0;
            EventBus<TestEvent>.Subscribe(e => received = e.Value);
            EventBus<TestEvent>.Publish(new TestEvent { Value = 42 });
            Assert.AreEqual(42, received, "Subscriber should receive the exact published value.");
        }

        [Test]
        public void Clear_RemovesAllSubscribers()
        {
            var called = false;
            EventBus<TestEvent>.Subscribe(_ => called = true);
            EventBus<TestEvent>.Clear();
            EventBus<TestEvent>.Publish(new TestEvent());
            Assert.IsFalse(called, "After Clear(), no handlers should be called.");
        }

        [Test]
        public void Subscribe_WithNullHandler_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EventBus<TestEvent>.Subscribe(null));
        }
    }
}
