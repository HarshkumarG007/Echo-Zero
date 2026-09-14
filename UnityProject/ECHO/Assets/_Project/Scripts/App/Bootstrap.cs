using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Settings;
using EchoZero.Data.Save;
using EchoZero.Data.Telemetry;
using EchoZero.Narrative;
using EchoZero.Core.ML;

namespace EchoZero.App
{
    /// <summary>
    /// Composition root for the game. Attached to a GameObject in BootstrapScene.
    ///
    /// Responsibilities:
    /// 1. Instantiates and registers ALL services into ServiceLocator.
    /// 2. Additively loads the first game scene after registration completes.
    ///
    /// Lives in EchoZero.App assembly so it can reference every layer
    /// without creating circular dependencies. This is the ONLY class
    /// allowed to depend on multiple layers simultaneously.
    ///
    /// DefaultExecutionOrder(-1000) guarantees this runs before all other scripts.
    ///
    /// TASK: TASK-003
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string _firstGameScene = "AerieUpperScene";

        private void Awake()
        {
            RegisterServices();
            Debug.Log("[Bootstrap][Info] All services registered.");
        }

        private void Start()
        {
            // Start fires after all Awake() calls complete — safe to load scenes here.
            if (ServiceLocator.TryGet<ISceneLoader>(out var loader))
                loader.LoadAdditiveAsync(_firstGameScene);
            else
                Debug.LogError("[Bootstrap][Error] ISceneLoader not registered.");
        }

        // ------------------------------------------------------------------ //
        // Service registration — one place, explicit order
        // ------------------------------------------------------------------ //

        private void RegisterServices()
        {
            // Core
            var config = new ConfigService();
            ServiceLocator.Register<IConfigService>(config);

            // Data
            var telemetry = new TelemetryService();
            var save      = new SaveService();
            ServiceLocator.Register<ITelemetryService>(telemetry);
            ServiceLocator.Register<ISaveService>(save);

            // World
            var sceneLoader = new SceneLoader(this);   // pass self as coroutine runner
            ServiceLocator.Register<ISceneLoader>(sceneLoader);
            
            var worldState = new EchoZero.Core.WorldState.WorldState();
            ServiceLocator.Register<EchoZero.Core.WorldState.WorldState>(worldState);

            // Narrative
            var narrativeState = new NarrativeState();
            ServiceLocator.Register<NarrativeState>(narrativeState);

            // Gameplay State
            var gameState = new GameStateManager();
            ServiceLocator.Register<GameStateManager>(gameState);

            // Settings (loads persisted prefs immediately)
            var settingsManager = new SettingsManager();
            ServiceLocator.Register<SettingsManager>(settingsManager);

            // ML
            var sentisRunner = new SentisModelRunner();
            ServiceLocator.Register<ISentisModelRunner>(sentisRunner);

            // Start telemetry session
            telemetry.TrackSessionStart();
        }
    }
}
