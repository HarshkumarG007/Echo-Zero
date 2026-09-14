using NUnit.Framework;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;

namespace EchoZero.Tests.EditMode
{
    public class ContradictionTests
    {
        private FragmentRegistry _registry;
        private MemoryFragmentSO _fragA;
        private MemoryFragmentSO _fragB;
        private MemoryFragmentSO _fragNeutral;

        [SetUp]
        public void SetUp()
        {
            _fragA = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            _fragA.fragmentId = "frag_A";
            _fragA.contradictsFragmentId = "frag_B";

            _fragB = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            _fragB.fragmentId = "frag_B";
            _fragB.contradictsFragmentId = "frag_A";

            _fragNeutral = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            _fragNeutral.fragmentId = "frag_Neutral";
            _fragNeutral.contradictsFragmentId = "";

            _registry = new FragmentRegistry(new[] { _fragA, _fragB, _fragNeutral });
        }

        [TearDown]
        public void TearDown()
        {
            _registry.Dispose();
            Object.DestroyImmediate(_fragA);
            Object.DestroyImmediate(_fragB);
            Object.DestroyImmediate(_fragNeutral);
        }

        [Test]
        public void CollectingSingleFragment_DoesNotTriggerContradiction()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            
            Assert.IsFalse(_registry.IsPlayerMustChooseState);
            Assert.IsTrue(_registry.IsCollected("frag_A"));
        }

        [Test]
        public void CollectingNonContradictingFragments_DoesNotTriggerContradiction()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_Neutral" });
            
            Assert.IsFalse(_registry.IsPlayerMustChooseState);
            Assert.IsTrue(_registry.IsCollected("frag_A"));
            Assert.IsTrue(_registry.IsCollected("frag_Neutral"));
        }

        [Test]
        public void CollectingContradictingFragments_TriggersPlayerMustChooseState()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_B" });
            
            Assert.IsTrue(_registry.IsPlayerMustChooseState);
            Assert.IsTrue(_registry.IsCollected("frag_A"));
            Assert.IsTrue(_registry.IsCollected("frag_B"));
        }

        [Test]
        public void ValidatingOneFragment_InvalidatesTheOther_AndResolvesState()
        {
            // Setup contradiction
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_B" });
            
            Assert.IsTrue(_registry.IsPlayerMustChooseState);

            // Validate A
            bool result = _registry.ValidateFragment("frag_A");
            
            Assert.IsTrue(result);
            Assert.IsFalse(_registry.IsPlayerMustChooseState);
            Assert.IsTrue(_registry.IsCollected("frag_A"));
            Assert.IsFalse(_registry.IsCollected("frag_B")); // B was invalidated
        }

        [Test]
        public void ValidatingUnrelatedFragment_WhenInChoiceState_Fails()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_B" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_Neutral" });
            
            // Try to validate neutral
            bool result = _registry.ValidateFragment("frag_Neutral");
            
            Assert.IsFalse(result);
            Assert.IsTrue(_registry.IsPlayerMustChooseState); // Contradiction still active
            Assert.IsTrue(_registry.IsCollected("frag_A"));
            Assert.IsTrue(_registry.IsCollected("frag_B"));
        }
    }
}
