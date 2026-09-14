using NUnit.Framework;
using UnityEngine;
using EchoZero.AI.Drift;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.Tests.EditMode
{
    public class DriftStateMachineTests
    {
        private DriftStateMachine _fsm;

        [SetUp]
        public void SetUp()
        {
            _fsm = new DriftStateMachine();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<DriftStabilizedEvent>.Clear();
        }

        [Test]
        public void InitialState_IsPatrol()
        {
            Assert.AreEqual(DriftState.Patrol, _fsm.CurrentState);
        }

        [Test]
        public void DistanceBelowDetection_TransitionsToAlerted()
        {
            _fsm.Update(0.1f, 10f, false);
            Assert.AreEqual(DriftState.Alerted, _fsm.CurrentState);
        }

        [Test]
        public void Alerted_TransitionsToPursuing_AfterPause()
        {
            _fsm.Update(0.1f, 10f, false); // Transitions to Alerted
            _fsm.Update(1.1f, 10f, false); // Pauses for 1s+, transitions to Pursuing

            Assert.AreEqual(DriftState.Pursuing, _fsm.CurrentState);
        }

        [Test]
        public void Pursuing_TransitionsToDestabilizing_WhenClose()
        {
            _fsm.Update(0.1f, 10f, false); // Patrol -> Alerted
            _fsm.Update(1.1f, 10f, false); // Alerted -> Pursuing
            _fsm.Update(0.1f, 2f, false);  // Pursuing -> Destabilizing

            Assert.AreEqual(DriftState.Destabilizing, _fsm.CurrentState);
        }

        [Test]
        public void SustainedRecall_TriggersStabilized_AndFiresEvent()
        {
            bool eventFired = false;
            EventBus<DriftStabilizedEvent>.Subscribe(e => eventFired = true);

            _fsm.Update(1.5f, 5f, true); // Being recalled
            Assert.AreEqual(DriftState.Patrol, _fsm.CurrentState); // Still in patrol initially

            _fsm.Update(1.6f, 5f, true); // Total recall time > 3s

            Assert.AreEqual(DriftState.Stabilized, _fsm.CurrentState);
            Assert.IsTrue(eventFired);
        }
    }
}
