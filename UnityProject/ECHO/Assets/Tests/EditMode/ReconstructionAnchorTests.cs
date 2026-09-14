using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Narrative.Fragments;
using EchoZero.World.Anchors;
using EchoZero.Narrative;
using System.Collections.Generic;

namespace EchoZero.Tests.EditMode
{
    public class ReconstructionAnchorTests
    {
        private GameObject _anchorGO;
        private ReconstructionAnchor _anchor;
        private ReconstructionAnchorSO _anchorData;
        private FragmentRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();

            _anchorData = ScriptableObject.CreateInstance<ReconstructionAnchorSO>();
            _anchorData.anchorId = "test_anchor";
            _anchorData.requiredFragmentIds = new[] { "frag_1", "frag_2" };

            _anchorGO = new GameObject();
            _anchor = _anchorGO.AddComponent<ReconstructionAnchor>();

            // Reflection to set private serialized field
            var field = typeof(ReconstructionAnchor).GetField("_anchorData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_anchor, _anchorData);

            // Mock FragmentRegistry by providing an empty list
            _registry = new FragmentRegistry(new List<MemoryFragmentSO>());
            ServiceLocator.Register<FragmentRegistry>(_registry);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            _registry.Dispose();
            Object.DestroyImmediate(_anchorGO);
            Object.DestroyImmediate(_anchorData);
            EventBus<AnchorRebuiltEvent>.Clear();
        }

        [Test]
        public void OnRecall_WhenFragmentsMissing_DoesNotFireEvent()
        {
            bool eventFired = false;
            EventBus<AnchorRebuiltEvent>.Subscribe(e => eventFired = true);

            _anchor.OnRecall();

            Assert.IsFalse(eventFired);
            // It should remain valid for recall if it didn't complete
            Assert.IsTrue(_anchor.CanRecall);
        }

        [Test]
        public void OnRecall_WhenAllFragmentsCollected_FiresEvent_AndDisablesRecall()
        {
            bool eventFired = false;
            EventBus<AnchorRebuiltEvent>.Subscribe(e => eventFired = true);

            // Simulate collection
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_1" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_2" });

            _anchor.OnRecall();

            Assert.IsTrue(eventFired);
            Assert.IsFalse(_anchor.CanRecall);
        }
    }
}
