using NUnit.Framework;
using NSubstitute;
using EchoZero.Narrative;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Unit tests for NarrativeState.
    /// TASK: TASK-006 (pre-authored alongside TASK-005)
    /// </summary>
    [TestFixture]
    public class NarrativeStateTests
    {
        private NarrativeState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new NarrativeState();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<NarrativeFlagSetEvent>.Clear();
        }

        [Test]
        public void SetFlag_ThenGetFlag_ReturnsSetValue()
        {
            _state.SetFlag("test_flag", 1);
            Assert.AreEqual(1, _state.GetFlag("test_flag"));
        }

        [Test]
        public void GetFlag_Unset_ReturnsZero()
        {
            Assert.AreEqual(0, _state.GetFlag("nonexistent"));
        }

        [Test]
        public void HasFlag_AfterSet_ReturnsTrue()
        {
            _state.SetFlag("test_flag");
            Assert.IsTrue(_state.HasFlag("test_flag"));
        }

        [Test]
        public void HasFlag_Unset_ReturnsFalse()
        {
            Assert.IsFalse(_state.HasFlag("missing"));
        }

        [Test]
        public void SetFlag_PublishesNarrativeFlagSetEvent()
        {
            NarrativeFlagSetEvent? received = null;
            EventBus<NarrativeFlagSetEvent>.Subscribe(e => received = e);
            _state.SetFlag("test_flag", 5);
            Assert.IsNotNull(received);
            Assert.AreEqual("test_flag", received.Value.FlagKey);
            Assert.AreEqual(5, received.Value.Value);
        }

        [Test]
        public void ToSaveData_ThenLoadFrom_RestoresAllFlags()
        {
            _state.SetFlag("flag_a", 1);
            _state.SetFlag("flag_b", 2);
            var data  = _state.ToSaveData();
            var state2 = new NarrativeState();
            state2.LoadFrom(data);
            Assert.AreEqual(1, state2.GetFlag("flag_a"));
            Assert.AreEqual(2, state2.GetFlag("flag_b"));
        }

        [Test]
        public void LoadFrom_WithNull_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _state.LoadFrom(null));
        }

        [Test]
        public void SetFlag_WithNullKey_IsIgnored()
        {
            Assert.DoesNotThrow(() => _state.SetFlag(null));
            Assert.AreEqual(0, _state.GetFlag(null));
        }
    }
}
