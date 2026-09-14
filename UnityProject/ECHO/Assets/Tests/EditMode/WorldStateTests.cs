using NUnit.Framework;
using EchoZero.Core.WorldState;

namespace EchoZero.Tests.EditMode
{
    public class WorldStateTests
    {
        private WorldState _worldState;

        [SetUp]
        public void SetUp()
        {
            _worldState = new WorldState();
        }

        [Test]
        public void TryGetFragment_WhenRegistered_ReturnsTrueAndCorrectId()
        {
            _worldState.RegisterFragment("object_123", "fragment_abc");

            bool found = _worldState.TryGetFragment("object_123", out string fragmentId);

            Assert.IsTrue(found);
            Assert.AreEqual("fragment_abc", fragmentId);
        }

        [Test]
        public void TryGetFragment_WhenNotRegistered_ReturnsFalse()
        {
            _worldState.RegisterFragment("object_123", "fragment_abc");

            bool found = _worldState.TryGetFragment("object_456", out string fragmentId);

            Assert.IsFalse(found);
            Assert.IsNull(fragmentId);
        }

        [Test]
        public void UnregisterFragment_RemovesFragmentFromState()
        {
            _worldState.RegisterFragment("object_123", "fragment_abc");
            _worldState.UnregisterFragment("object_123");

            bool found = _worldState.TryGetFragment("object_123", out string fragmentId);

            Assert.IsFalse(found);
            Assert.IsNull(fragmentId);
        }
    }
}
