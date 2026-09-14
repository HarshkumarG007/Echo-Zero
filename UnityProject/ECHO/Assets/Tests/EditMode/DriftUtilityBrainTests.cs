using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.AI.Drift;

namespace EchoZero.Tests.EditMode
{
    public class DriftUtilityBrainTests
    {
        private DriftUtilityBrain _brain;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _brain = new DriftUtilityBrain();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<DriftStabilizedEvent>.Clear();
        }

        [Test]
        public void InitialState_IsPatrol()
        {
            Assert.AreEqual(DriftState.Patrol, _brain.CurrentState);
        }

        [Test]
        public void Update_WhenPlayerClose_TransitionsToPursue()
        {
            _brain.Update(0.1f, 10f, false);
            Assert.AreEqual(DriftState.Pursuing, _brain.CurrentState);
        }

        [Test]
        public void Update_WhenPlayerVeryClose_TransitionsToDestabilizing()
        {
            _brain.Update(0.1f, 2f, false);
            Assert.AreEqual(DriftState.Destabilizing, _brain.CurrentState);
        }

        [Test]
        public void Update_WhenRecalledForDuration_TransitionsToStabilized()
        {
            bool eventFired = false;
            EventBus<DriftStabilizedEvent>.Subscribe(e => eventFired = true);

            _brain.Update(1f, 20f, true);
            _brain.Update(1f, 20f, true);
            _brain.Update(1.5f, 20f, true); // Total 3.5s

            Assert.AreEqual(DriftState.Stabilized, _brain.CurrentState);
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void Update_WhenPlayerMovesAway_TransitionsBackToPatrol()
        {
            _brain.Update(0.1f, 10f, false); // Pursue
            _brain.Update(0.1f, 20f, false); // Move far away
            
            Assert.AreEqual(DriftState.Patrol, _brain.CurrentState);
        }
    }
}
