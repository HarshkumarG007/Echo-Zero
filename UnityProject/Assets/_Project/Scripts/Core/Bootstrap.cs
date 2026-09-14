using UnityEngine;
using EchoZero.Data.Save;
using EchoZero.Data.Telemetry;

namespace EchoZero.Core
{
    /// <summary>
    /// Entry point for the game. Attached to a GameObject in BootstrapScene.
    ///
    /// Responsibilities:
    /// 1. Registers all core services into ServiceLocator before any other MonoBehaviour runs.
    /// 2. Loads AerieUpperScene additively after initialisation is complete.
    ///
    /// This MonoBehaviour uses DefaultExecutionOrder(-1000) to guarantee it runs
    /// before all other scripts in the same frame.
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
            LogBootstrap();
        }

        private void Start()
        {
            // Start is used instead of Awake for scene loading so that all
            // Awake() calls in Bootstrap complete before any scene loads.
            if (ServiceLocator.TryGet<ISceneLoader>(out var loader))
                loader.LoadAdditiveAsync(_firstGameScene);
        }

        // ------------------------------------------------------------------ //
        // Service Registration
        // ------------------------------------------------------------------ //

        private void RegisterServices()
        {
            // Data layer — registered first (no dependencies)
            var config    = new ConfigService();
            var telemetry = new TelemetryService();
            var save      = new SaveService();

            ServiceLocator.Register<IConfigService>(config);
            ServiceLocator.Register<ITelemetryService>(telemetry);
            ServiceLocator.Register<ISaveService>(save);

            // World layer
            var sceneLoader = new SceneLoader();
            ServiceLocator.Register<ISceneLoader>(sceneLoader);

            // Narrative layer
            var narrativeState = new Narrative.NarrativeState();
            ServiceLocator.Register<Narrative.NarrativeState>(narrativeState);

            // Telemetry: session start
            telemetry.TrackSessionStart();
        }

        private static void LogBootstrap()
        {
            Debug.Log("[Bootstrap][Info] All services registered. Loading first scene.");
        }
    }
}
