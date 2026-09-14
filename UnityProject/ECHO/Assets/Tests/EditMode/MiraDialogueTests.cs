using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;
using EchoZero.Narrative.Mira;

namespace EchoZero.Tests.EditMode
{
    public class MiraDialogueTests
    {
        private NarrativeState _narrativeState;
        private FragmentRegistry _fragmentRegistry;
        private MiraDialogueSelector _selector;

        [SetUp]
        public void SetUp()
        {
            _narrativeState = new NarrativeState();
            
            // Need at least two contradicting fragments to trigger IsPlayerMustChooseState
            var fragA = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragA.fragmentId = "frag_A";
            fragA.contradictsFragmentId = "frag_B";

            var fragB = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragB.fragmentId = "frag_B";
            fragB.contradictsFragmentId = "frag_A";

            _fragmentRegistry = new FragmentRegistry(new[] { fragA, fragB });
            _selector = new MiraDialogueSelector(_narrativeState, _fragmentRegistry);
        }

        [TearDown]
        public void TearDown()
        {
            _fragmentRegistry.Dispose();
            EventBus<FragmentCollectedEvent>.Clear();
        }

        [Test]
        public void GetCurrentDialogue_AtStart_ReturnsOpeningLine()
        {
            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("You're awake. We have work to do if we're going to fix this place.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_WithFragment01_ReturnsFragmentLine()
        {
            _narrativeState.SetFlag(NarrativeFlags.Fragment01Collected, 1);
            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("You found a piece of it. There is more out there.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_WithWalkwayRebuilt_ReturnsWalkwayLine()
        {
            // Walkway rebuilt takes precedence over Fragment01
            _narrativeState.SetFlag(NarrativeFlags.Fragment01Collected, 1);
            _narrativeState.SetFlag(NarrativeFlags.WalkwayRebuilt, 1);
            
            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("The path is clear now. You're getting closer.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_WithActiveContradiction_ReturnsContradictionLine()
        {
            _narrativeState.SetFlag(NarrativeFlags.WalkwayRebuilt, 1); // Contradiction takes precedence

            // Trigger contradiction in registry
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_A" });
            EventBus<FragmentCollectedEvent>.Publish(new FragmentCollectedEvent { FragmentId = "frag_B" });

            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("Two memories. They cannot both be true. You must decide what happened.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_AfterChoiceA_ReturnsChoiceALine()
        {
            _narrativeState.SetFlag(NarrativeFlags.ChoiceMade, 1);
            _narrativeState.SetFlag(NarrativeFlags.ChoiceFragmentA, 1);

            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("I remember leaving. It was my choice to go.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_AfterChoiceB_ReturnsChoiceBLine()
        {
            _narrativeState.SetFlag(NarrativeFlags.ChoiceMade, 1);
            // Notice ChoiceFragmentA is NOT set

            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("I remember fading. I didn't want to go.", dialogue);
        }

        [Test]
        public void GetCurrentDialogue_AfterReveal_ReturnsEmptyString()
        {
            // Reveal takes ultimate precedence
            _narrativeState.SetFlag(NarrativeFlags.RevealTriggered, 1);
            _narrativeState.SetFlag(NarrativeFlags.ChoiceMade, 1);

            string dialogue = _selector.GetCurrentDialogue();
            Assert.AreEqual("", dialogue);
        }
    }
}
