using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;
using EchoZero.Narrative.Mira;
using NSubstitute;

namespace EchoZero.Tests.EditMode
{
    public class MiraDialogueSelectorTests
    {
        private NarrativeState _narrativeState;
        private FragmentRegistry _fragmentRegistry;
        private MiraDialogueConfig _config;
        private ITelemetryService _mockTelemetry;
        private MiraDialogueSelector _selector;

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Clear();
            _mockTelemetry = Substitute.For<ITelemetryService>();
            ServiceLocator.Register<ITelemetryService>(_mockTelemetry);

            _narrativeState = new NarrativeState();
            _fragmentRegistry = new FragmentRegistry(System.Array.Empty<MemoryFragmentSO>());
            _config = ScriptableObject.CreateInstance<MiraDialogueConfig>();

            // Setup identifiable lines in config
            _config.DefaultLine = "DEFAULT";
            _config.DefaultLineAggressive = "AGGRESSIVE";
            _config.DefaultLineExplorer = "EXPLORER";

            _selector = new MiraDialogueSelector(_narrativeState, _fragmentRegistry, _config);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            _fragmentRegistry?.Dispose();
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void GetCurrentDialogue_AggressiveScore_ReturnsAggressiveVariant()
        {
            _mockTelemetry.GetAggressionScore().Returns(0.8f);
            
            string result = _selector.GetCurrentDialogue();
            
            Assert.AreEqual("AGGRESSIVE", result);
        }

        [Test]
        public void GetCurrentDialogue_ExplorerScore_ReturnsExplorerVariant()
        {
            _mockTelemetry.GetAggressionScore().Returns(0.2f);
            
            string result = _selector.GetCurrentDialogue();
            
            Assert.AreEqual("EXPLORER", result);
        }

        [Test]
        public void GetCurrentDialogue_NeutralScore_ReturnsDefaultVariant()
        {
            _mockTelemetry.GetAggressionScore().Returns(0.5f);
            
            string result = _selector.GetCurrentDialogue();
            
            Assert.AreEqual("DEFAULT", result);
        }

        [Test]
        public void GetCurrentDialogue_AggressiveScoreButNoVariant_ReturnsDefault()
        {
            _config.DefaultLineAggressive = ""; // Clear aggressive variant
            _mockTelemetry.GetAggressionScore().Returns(0.8f);
            
            string result = _selector.GetCurrentDialogue();
            
            Assert.AreEqual("DEFAULT", result);
        }
    }
}
