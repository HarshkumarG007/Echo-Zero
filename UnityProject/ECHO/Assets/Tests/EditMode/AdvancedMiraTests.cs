using NUnit.Framework;
using System.Collections.Generic;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;
using EchoZero.Narrative.Mira;

namespace EchoZero.Tests.EditMode
{
    public class AdvancedMiraTests
    {
        private NarrativeState _narrativeState;
        private FragmentRegistry _fragmentRegistry;
        private MiraDialogueSelector _mira;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _narrativeState = new NarrativeState();
            
            var fragments = new List<MemoryFragmentSO>();
            var fragA = UnityEngine.ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragA.fragmentId = "Fragment_A";
            fragA.contradictsFragmentId = "Fragment_B";

            var fragB = UnityEngine.ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragB.fragmentId = "Fragment_B";
            fragB.contradictsFragmentId = "Fragment_A";

            fragments.Add(fragA);
            fragments.Add(fragB);

            _fragmentRegistry = new FragmentRegistry(fragments);
            _mira = new MiraDialogueSelector(_narrativeState, _fragmentRegistry);
        }

        [TearDown]
        public void TearDown()
        {
            _fragmentRegistry.Dispose();
            ServiceLocator.Clear();
        }

        [Test]
        public void GetCurrentDialogue_ReactsToLastCollectedFragment_A()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "Fragment_A" });
            string dialogue = _mira.GetCurrentDialogue();
            
            Assert.AreEqual("That memory... it feels heavy. Are you sure it's yours?", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_ReactsToLastCollectedFragment_B()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "Fragment_B" });
            string dialogue = _mira.GetCurrentDialogue();
            
            Assert.AreEqual("A bright shard. It almost hurts to look at.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_ReturnsContradictionDialogue_WhenBothCollected()
        {
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "Fragment_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "Fragment_B" });
            
            string dialogue = _mira.GetCurrentDialogue();
            
            Assert.AreEqual("Two memories. They cannot both be true. You must decide what happened.", dialogue);
        }
    }
}
