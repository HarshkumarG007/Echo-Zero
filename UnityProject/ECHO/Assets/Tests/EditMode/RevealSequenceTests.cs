using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Narrative;
using EchoZero.Narrative.Mira;

namespace EchoZero.Tests.EditMode
{
    public class RevealSequenceTests
    {
        private NarrativeState _narrativeState;
        private GameObject _miraGO;
        private GameObject _hudGO;
        private RevealSequence _revealSequence;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _narrativeState = new NarrativeState();
            ServiceLocator.Register<NarrativeState>(_narrativeState);

            _miraGO = new GameObject("Mira");
            // MiraDialogueSelector is a plain C# class, not MonoBehaviour.
            // RevealSequence disables its *host* GameObject, so we just pass the GO.
            _hudGO = new GameObject("HUD");

            var revealGO = new GameObject("RevealTrigger");
            _revealSequence = revealGO.AddComponent<RevealSequence>();

            // Setup serialized fields via reflection
            var miraField = typeof(RevealSequence).GetField("_miraGameObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            miraField?.SetValue(_revealSequence, _miraGO);

            var hudField = typeof(RevealSequence).GetField("_playerHUD", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hudField?.SetValue(_revealSequence, _hudGO);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            Object.DestroyImmediate(_miraGO);
            Object.DestroyImmediate(_hudGO);
            Object.DestroyImmediate(_revealSequence.gameObject);
        }

        [Test]
        public void TriggerReveal_SetsFlag_AndDisablesObjects()
        {
            Assert.IsFalse(_narrativeState.HasFlag(NarrativeFlags.RevealTriggered));
            Assert.IsTrue(_miraGO.activeSelf);
            Assert.IsTrue(_hudGO.activeSelf);

            _revealSequence.TriggerReveal();

            Assert.IsTrue(_narrativeState.HasFlag(NarrativeFlags.RevealTriggered));
            Assert.IsFalse(_miraGO.activeSelf);
            Assert.IsFalse(_hudGO.activeSelf);
        }
    }
}
