using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;
using EchoZero.Narrative;
using EchoZero.Narrative.Fragments;
using EchoZero.AI.Drift;

namespace EchoZero.Tests.PlayMode
{
    /// <summary>
    /// Automated end-to-end integration tests in PlayMode.
    /// Verifies Unity physics triggers, CharacterController interaction,
    /// EventBus dispatch, and full narrative progression across frames.
    /// TASK: TASK-040
    /// </summary>
    public class PlayModeWalkthroughTests
    {
        private class DummyTelemetryService : ITelemetryService
        {
            public int FragmentsCollected { get; private set; }
            public int ChoicesMade { get; private set; }

            public void TrackEvent(string eventName, Dictionary<string, object> parameters = null) { }
            public void TrackFragmentCollected(string fragmentId) => FragmentsCollected++;
            public void TrackChoiceMade(string fragmentId) => ChoicesMade++;
            public void TrackSessionStart() { }
            public void TrackSessionEnd() { }
            public float GetAggressionScore() => 0f;
        }

        private readonly List<GameObject> _spawnedObjects = new();
        private WorldState _worldState;
        private NarrativeState _narrativeState;
        private DummyTelemetryService _telemetryService;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _spawnedObjects.Clear();

            _worldState = new WorldState();
            _narrativeState = new NarrativeState();
            _telemetryService = new DummyTelemetryService();

            ServiceLocator.Register<WorldState>(_worldState);
            ServiceLocator.Register<NarrativeState>(_narrativeState);
            ServiceLocator.Register<ITelemetryService>(_telemetryService);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var go in _spawnedObjects)
            {
                if (go != null) Object.Destroy(go);
            }
            _spawnedObjects.Clear();

            ServiceLocator.Clear();
            EventBus<FragmentCollectedEvent>.Clear();
            EventBus<ChoiceMadeEvent>.Clear();
            EventBus<DriftStabilizedEvent>.Clear();
        }

        private GameObject CreatePlayer(Vector3 position)
        {
            var player = new GameObject("Player_ZERO");
            player.tag = "Player";
            player.transform.position = position;

            var cc = player.AddComponent<CharacterController>();
            cc.radius = 0.5f;
            cc.height = 2f;
            cc.center = new Vector3(0, 1, 0);

            _spawnedObjects.Add(player);
            return player;
        }

        [UnityTest]
        public IEnumerator PlayMode_FragmentPickup_OnPlayerCollision_CollectsAndDispatchesEvent()
        {
            bool eventFired = false;
            string collectedId = null;

            EventBus<FragmentCollectedEvent>.Subscribe(evt =>
            {
                eventFired = true;
                collectedId = evt.FragmentId;
            });

            // 1. Create Fragment SO
            var fragmentSO = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragmentSO.fragmentId = "frag_test_walkthrough";
            fragmentSO.narrativeFlagOnCollect = "Flag_TestCollected";

            // 2. Spawn Fragment Pickup in world
            var pickupObj = new GameObject("FragmentPickup");
            pickupObj.transform.position = new Vector3(0, 0, 1.5f);

            var col = pickupObj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(1.5f, 1.5f, 1.5f);

            // Kinematic Rigidbody is required for trigger events with CharacterController
            var rb = pickupObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var pickup = pickupObj.AddComponent<MemoryFragmentPickup>();

            // Assign fragment via reflection for serialized field
            var fragmentField = typeof(MemoryFragmentPickup).GetField("_fragment",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fragmentField?.SetValue(pickup, fragmentSO);

            _spawnedObjects.Add(pickupObj);

            // 3. Spawn Player at origin
            var player = CreatePlayer(Vector3.zero);
            var cc = player.GetComponent<CharacterController>();

            // 4. Yield one frame to allow Unity Awake/Start initialization
            yield return null;

            // 5. Move player toward fragment through physics updates
            for (int i = 0; i < 20; i++)
            {
                cc.Move(new Vector3(0, 0, 0.2f));
                yield return new WaitForFixedUpdate();
            }

            // Assertions
            Assert.IsTrue(eventFired, "FragmentCollectedEvent must be fired upon collision with pickup.");
            Assert.AreEqual("frag_test_walkthrough", collectedId);
            Assert.IsTrue(_narrativeState.GetFlag("Flag_TestCollected"), "Narrative flag must be set.");
            Assert.AreEqual(1, _telemetryService.FragmentsCollected, "Telemetry must record fragment collection.");
            Assert.IsFalse(pickup.CanRecall, "Collected fragment must no longer be recallable.");
        }

        [UnityTest]
        public IEnumerator PlayMode_ChoicePedestal_WhenContradictionActive_ValidatesChoiceAndLocksAlternate()
        {
            bool choiceEventFired = false;
            string chosenId = null;

            EventBus<ChoiceMadeEvent>.Subscribe(evt =>
            {
                choiceEventFired = true;
                chosenId = evt.ChosenFragmentId;
            });

            // 1. Create contradictory fragments
            var fragA = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragA.fragmentId = "frag_choice_A";

            var fragB = ScriptableObject.CreateInstance<MemoryFragmentSO>();
            fragB.fragmentId = "frag_choice_B";
            fragB.contradictsFragmentId = "frag_choice_A";

            // 2. Set up FragmentRegistry in ServiceLocator
            var registry = new FragmentRegistry(new[] { fragA, fragB });
            ServiceLocator.Register<FragmentRegistry>(registry);

            // Collect both to enter contradiction state
            registry.RegisterFoundFragment("frag_choice_A");
            registry.RegisterFoundFragment("frag_choice_B");
            Assert.IsTrue(registry.IsPlayerMustChooseState, "Registry must be in player choice state.");

            // 3. Spawn Pedestals
            var pedAObj = new GameObject("Pedestal_A");
            var pedBObj = new GameObject("Pedestal_B");
            _spawnedObjects.Add(pedAObj);
            _spawnedObjects.Add(pedBObj);

            var pedA = pedAObj.AddComponent<ChoicePedestal>();
            var pedB = pedBObj.AddComponent<ChoicePedestal>();

            // Wire serialized fields via reflection
            var assignedFragField = typeof(ChoicePedestal).GetField("_assignedFragment",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var alternateField = typeof(ChoicePedestal).GetField("_alternatePedestal",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            assignedFragField?.SetValue(pedA, fragA);
            alternateField?.SetValue(pedA, pedB);

            assignedFragField?.SetValue(pedB, fragB);
            alternateField?.SetValue(pedB, pedA);

            yield return null;

            // 4. Player uses RECALL on Pedestal A
            pedA.OnRecall();

            // Assertions
            Assert.IsTrue(choiceEventFired, "ChoiceMadeEvent must be published on validation.");
            Assert.AreEqual("frag_choice_A", chosenId);
            Assert.IsFalse(pedA.CanRecall, "Validated pedestal must be locked.");
            Assert.IsFalse(pedB.CanRecall, "Alternate pedestal must be locked.");
            Assert.IsTrue(registry.IsFragmentValidated("frag_choice_A"));
            Assert.IsFalse(registry.IsFragmentValidated("frag_choice_B"));
        }

        [UnityTest]
        public IEnumerator PlayMode_DriftEncounter_SustainedRecall_TransitionsToStabilized()
        {
            bool stabilizedEventFired = false;
            EventBus<DriftStabilizedEvent>.Subscribe(_ => stabilizedEventFired = true);

            var player = CreatePlayer(Vector3.zero);

            var driftObj = new GameObject("TheDrift");
            driftObj.transform.position = new Vector3(0, 0, 5f); // 5m away: within pursuit range
            _spawnedObjects.Add(driftObj);

            var drift = driftObj.AddComponent<DriftController>();
            drift.SetPlayerTransform(player.transform);

            yield return null; // Allow Awake/Start

            // Assert initial state is pursuing (distance is 5m < 15m)
            drift.ForceUpdate(0.016f, 5f, false);
            Assert.AreEqual(DriftState.Pursuing, drift.CurrentState);

            // Simulate player sustaining RECALL for 3.2 seconds
            float elapsed = 0f;
            while (elapsed < 3.2f)
            {
                drift.OnRecall();
                drift.ForceUpdate(0.1f, 5f, true);
                elapsed += 0.1f;
                yield return null;
            }

            // Assert stabilized terminal state
            Assert.AreEqual(DriftState.Stabilized, drift.CurrentState);
            Assert.IsTrue(stabilizedEventFired, "DriftStabilizedEvent must be published upon stabilization.");
            Assert.IsFalse(drift.CanRecall, "Stabilized Drift cannot be recalled again.");
        }

        [UnityTest]
        public IEnumerator PlayMode_RevealSequence_OnTriggerEnter_SetsRevealFlagAndHidesHUD()
        {
            // 1. Spawn HUD and Mira mock objects
            var hudObj = new GameObject("PlayerHUD");
            var miraObj = new GameObject("MiraNPC");
            _spawnedObjects.Add(hudObj);
            _spawnedObjects.Add(miraObj);

            // 2. Spawn Reveal Volume
            var revealObj = new GameObject("RevealTrigger");
            revealObj.transform.position = new Vector3(0, 0, 2f);
            _spawnedObjects.Add(revealObj);

            var col = revealObj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(2f, 2f, 2f);

            var rb = revealObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var reveal = revealObj.AddComponent<RevealSequence>();

            // Wire serialized fields
            var hudField = typeof(RevealSequence).GetField("_playerHUD",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var miraField = typeof(RevealSequence).GetField("_miraGameObject",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            hudField?.SetValue(reveal, hudObj);
            miraField?.SetValue(reveal, miraObj);

            // 3. Spawn Player
            var player = CreatePlayer(Vector3.zero);
            var cc = player.GetComponent<CharacterController>();

            yield return null;

            // 4. Move player into reveal trigger volume
            for (int i = 0; i < 20; i++)
            {
                cc.Move(new Vector3(0, 0, 0.2f));
                yield return new WaitForFixedUpdate();
            }

            // Assertions
            Assert.IsTrue(_narrativeState.GetFlag(NarrativeFlags.RevealTriggered),
                "RevealTriggered flag must be set in NarrativeState.");
            Assert.IsFalse(hudObj.activeSelf, "Player HUD must be hidden upon reveal.");
            Assert.IsFalse(miraObj.activeSelf, "Mira NPC must be deactivated upon reveal.");
        }
    }
}
