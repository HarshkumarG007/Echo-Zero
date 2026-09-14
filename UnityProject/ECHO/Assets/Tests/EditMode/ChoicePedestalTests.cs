using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;

namespace EchoZero.Tests.EditMode
{
    public class ChoicePedestalTests
    {
        private GameObject _pedestalGO1;
        private GameObject _pedestalGO2;
        private ChoicePedestal _pedestal1;
        private ChoicePedestal _pedestal2;
        private FragmentRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();

            var fragA = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragA.fragmentId = "frag_A";
            fragA.contradictsFragmentId = "frag_B";

            var fragB = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragB.fragmentId = "frag_B";
            fragB.contradictsFragmentId = "frag_A";

            _registry = new FragmentRegistry(new[] { fragA, fragB });
            ServiceLocator.Register<FragmentRegistry>(_registry);

            // Simulate collecting both fragments to enter contradiction state
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_B" });

            _pedestalGO1 = new GameObject();
            _pedestalGO2 = new GameObject();

            _pedestal1 = _pedestalGO1.AddComponent<ChoicePedestal>();
            _pedestal2 = _pedestalGO2.AddComponent<ChoicePedestal>();

            // Reflection to set serialized fields
            var assignFragField = typeof(ChoicePedestal).GetField("_assignedFragment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var alternateField = typeof(ChoicePedestal).GetField("_alternatePedestal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            assignFragField.SetValue(_pedestal1, fragA);
            alternateField.SetValue(_pedestal1, _pedestal2);

            assignFragField.SetValue(_pedestal2, fragB);
            alternateField.SetValue(_pedestal2, _pedestal1);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            _registry.Dispose();
            Object.DestroyImmediate(_pedestalGO1);
            Object.DestroyImmediate(_pedestalGO2);
            EventBus<ChoiceMadeEvent>.Clear();
            EventBus<FragmentCollectedEvent>.Clear();
        }

        [Test]
        public void OnRecall_ValidatesFragment_AndLocksBothPedestals()
        {
            bool eventFired = false;
            EventBus<ChoiceMadeEvent>.Subscribe(e =>
            {
                eventFired = true;
                Assert.AreEqual("frag_A", e.ChosenFragmentId);
            });

            Assert.IsTrue(_registry.IsPlayerMustChooseState);
            Assert.IsTrue(_pedestal1.CanRecall);
            Assert.IsTrue(_pedestal2.CanRecall);

            _pedestal1.OnRecall();

            Assert.IsTrue(eventFired);
            Assert.IsFalse(_registry.IsPlayerMustChooseState); // Contradiction resolved
            Assert.IsTrue(_registry.IsCollected("frag_A"));
            Assert.IsFalse(_registry.IsCollected("frag_B"));

            Assert.IsFalse(_pedestal1.CanRecall);
            Assert.IsFalse(_pedestal2.CanRecall);
        }
    }
}
