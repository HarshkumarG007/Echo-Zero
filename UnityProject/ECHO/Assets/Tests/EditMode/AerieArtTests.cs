using NUnit.Framework;
using UnityEngine;
using EchoZero.Tools.Editor;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Unit tests verifying procedural PBR materials and compound mesh generation.
    /// Follows ADR-0007 (Procedural PBR Asset Architecture).
    /// TASK: TASK-041
    /// </summary>
    public class AerieArtTests
    {
        private GameObject _testRoot;

        [SetUp]
        public void SetUp()
        {
            _testRoot = new GameObject("AerieArtTestRoot");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testRoot != null)
            {
                Object.DestroyImmediate(_testRoot);
            }
        }

        [Test]
        public void PaletteColors_MatchDesignSystemSpecifications()
        {
            // Verify hex/color specifications from docs/design.md
            // Crystal Cyan: #64C8FF (0.39, 0.78, 1.0)
            Assert.AreEqual(0.39f, AerieArtFactory.Palette.CrystalCyan.r, 0.02f);
            Assert.AreEqual(0.78f, AerieArtFactory.Palette.CrystalCyan.g, 0.02f);
            Assert.AreEqual(1.0f, AerieArtFactory.Palette.CrystalCyan.b, 0.02f);

            // Drift Amber: #FF6B4A (1.0, 0.42, 0.29)
            Assert.AreEqual(1.0f, AerieArtFactory.Palette.DriftAmber.r, 0.02f);
            Assert.AreEqual(0.42f, AerieArtFactory.Palette.DriftAmber.g, 0.02f);
            Assert.AreEqual(0.29f, AerieArtFactory.Palette.DriftAmber.b, 0.02f);

            // Pedestal Gold: #FFD264 (1.0, 0.82, 0.39)
            Assert.AreEqual(1.0f, AerieArtFactory.Palette.PedestalGold.r, 0.02f);
            Assert.AreEqual(0.82f, AerieArtFactory.Palette.PedestalGold.g, 0.02f);
            Assert.AreEqual(0.39f, AerieArtFactory.Palette.PedestalGold.b, 0.02f);
        }

        [Test]
        public void CreateMemoryCrystalVisual_SpawnsHierarchicalCompoundObject()
        {
            var crystalObj = AerieArtFactory.CreateMemoryCrystalVisual(_testRoot.transform);

            Assert.IsNotNull(crystalObj);
            Assert.AreEqual("Crystal_VisualCompound", crystalObj.name);

            // Should have 3 child components (Core_Diamond, Upper_Facet, Lower_Facet)
            Assert.AreEqual(3, crystalObj.transform.childCount);
            Assert.IsNotNull(crystalObj.transform.Find("Core_Diamond"));
            Assert.IsNotNull(crystalObj.transform.Find("Upper_Facet"));
            Assert.IsNotNull(crystalObj.transform.Find("Lower_Facet"));

            // Visual sub-meshes must not contain physics colliders (only visual renderers)
            var colliders = crystalObj.GetComponentsInChildren<Collider>();
            Assert.AreEqual(0, colliders.Length, "Visual compound meshes must not contain physics colliders.");
        }

        [Test]
        public void CreateDriftVisual_SpawnsCoreAndCarapace()
        {
            var driftVisual = AerieArtFactory.CreateDriftVisual(_testRoot.transform);

            Assert.IsNotNull(driftVisual);
            Assert.AreEqual("Drift_VisualCompound", driftVisual.name);

            // Must contain central core and 4 surrounding carapace shards
            Assert.IsNotNull(driftVisual.transform.Find("Drift_Core"));
            Assert.AreEqual(5, driftVisual.transform.childCount);

            var colliders = driftVisual.GetComponentsInChildren<Collider>();
            Assert.AreEqual(0, colliders.Length, "Visual compound meshes must not contain physics colliders.");
        }

        [Test]
        public void CreatePedestalVisual_SpawnsSteppedPlinth()
        {
            var pedestalVisual = AerieArtFactory.CreatePedestalVisual(_testRoot.transform, "TestPedestal", false);

            Assert.IsNotNull(pedestalVisual);
            Assert.AreEqual("TestPedestal_VisualCompound", pedestalVisual.name);

            Assert.IsNotNull(pedestalVisual.transform.Find("Plinth_Base"));
            Assert.IsNotNull(pedestalVisual.transform.Find("Plinth_Pillar"));
            Assert.IsNotNull(pedestalVisual.transform.Find("Receptacle_Ring"));

            var colliders = pedestalVisual.GetComponentsInChildren<Collider>();
            Assert.AreEqual(0, colliders.Length, "Visual compound meshes must not contain physics colliders.");
        }
    }
}
