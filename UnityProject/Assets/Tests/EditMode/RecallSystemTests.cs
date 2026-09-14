using NUnit.Framework;
using NSubstitute;
using EchoZero.Gameplay.Recall;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Gameplay;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Unit tests for RecallSystem.
    /// TASK: TASK-004
    /// </summary>
    [TestFixture]
    public class RecallSystemTests
    {
        private IConfigService _mockConfig;
        private RecallSystem   _system;

        [SetUp]
        public void SetUp()
        {
            _mockConfig = Substitute.For<IConfigService>();
            _mockConfig.RecallRange.Returns(4f);
            _system = new RecallSystem(_mockConfig);
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<RecallAttemptedEvent>.Clear();
            EventBus<RecallCompletedEvent>.Clear();
        }

        [Test]
        public void TryRecall_WithValidRecallable_CallsOnRecall()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(true);
            _system.TryRecall(target);
            target.Received(1).OnRecall();
        }

        [Test]
        public void TryRecall_WithValidRecallable_ReturnsTrue()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(true);
            var result = _system.TryRecall(target);
            Assert.IsTrue(result);
        }

        [Test]
        public void TryRecall_WithCanRecallFalse_DoesNotCallOnRecall()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(false);
            _system.TryRecall(target);
            target.DidNotReceive().OnRecall();
        }

        [Test]
        public void TryRecall_WithCanRecallFalse_ReturnsFalse()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(false);
            var result = _system.TryRecall(target);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryRecall_WithNull_ReturnsFalse()
        {
            var result = _system.TryRecall(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryRecall_WithNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _system.TryRecall(null));
        }

        [Test]
        public void TryRecall_Success_PublishesRecallCompletedEventWithSuccessTrue()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(true);

            RecallCompletedEvent? received = null;
            EventBus<RecallCompletedEvent>.Subscribe(e => received = e);

            _system.TryRecall(target);

            Assert.IsNotNull(received, "RecallCompletedEvent must be published.");
            Assert.IsTrue(received.Value.Success);
        }

        [Test]
        public void TryRecall_Fail_PublishesRecallCompletedEventWithSuccessFalse()
        {
            var target = Substitute.For<IRecallable>();
            target.CanRecall.Returns(false);

            RecallCompletedEvent? received = null;
            EventBus<RecallCompletedEvent>.Subscribe(e => received = e);

            _system.TryRecall(target);

            Assert.IsNotNull(received);
            Assert.IsFalse(received.Value.Success);
        }

        [Test]
        public void Constructor_WithNullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => new RecallSystem(null));
        }
    }
}
