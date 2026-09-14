using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EchoZero.Core.Events;
using EchoZero.Core.Events.World;

namespace EchoZero.Core
{
    /// <summary>Interface for scene loading. Allows test doubles to be substituted.</summary>
    public interface ISceneLoader
    {
        /// <summary>Loads a scene additively without unloading any existing scenes.</summary>
        void LoadAdditiveAsync(string sceneName);

        /// <summary>Unloads an additively loaded scene.</summary>
        void UnloadAsync(string sceneName);
    }

    /// <summary>
    /// Manages additive scene loading for the game.
    /// All scene transitions use additive mode — single-mode loads are forbidden after Bootstrap.
    ///
    /// Publishes SceneLoadedEvent and SceneUnloadedEvent via EventBus on completion.
    ///
    /// TASK: TASK-003
    /// </summary>
    public class SceneLoader : ISceneLoader
    {
        private readonly MonoBehaviour _coroutineRunner;

        /// <summary>
        /// SceneLoader needs a MonoBehaviour to run coroutines.
        /// In production, Bootstrap passes itself. In tests, use a test MonoBehaviour.
        /// </summary>
        public SceneLoader(MonoBehaviour coroutineRunner = null)
        {
            // If no runner supplied, SceneLoader will find Bootstrap in scene (test contexts
            // should always supply one explicitly to avoid FindObjectOfType).
            _coroutineRunner = coroutineRunner;
        }

        /// <inheritdoc/>
        public void LoadAdditiveAsync(string sceneName)
        {
            if (_coroutineRunner == null)
            {
                Debug.LogError("[SceneLoader][Error] No coroutine runner available. " +
                               "Ensure Bootstrap passes itself to SceneLoader.");
                return;
            }
            _coroutineRunner.StartCoroutine(LoadRoutine(sceneName));
        }

        /// <inheritdoc/>
        public void UnloadAsync(string sceneName)
        {
            if (_coroutineRunner == null)
            {
                Debug.LogError("[SceneLoader][Error] No coroutine runner available.");
                return;
            }
            _coroutineRunner.StartCoroutine(UnloadRoutine(sceneName));
        }

        // ------------------------------------------------------------------ //
        // Private coroutines
        // ------------------------------------------------------------------ //

        private IEnumerator LoadRoutine(string sceneName)
        {
            var startTime = Time.realtimeSinceStartup;
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (op == null)
            {
                Debug.LogError($"[SceneLoader][Error] Scene '{sceneName}' not found in Build Settings.");
                yield break;
            }

            yield return op;

            var durationMs = (Time.realtimeSinceStartup - startTime) * 1000f;
            Debug.Log($"[SceneLoader][Info] Loaded '{sceneName}' in {durationMs:F0}ms.");

            EventBus<SceneLoadedEvent>.Publish(new SceneLoadedEvent
            {
                SceneName     = sceneName,
                LoadDurationMs = durationMs
            });
        }

        private IEnumerator UnloadRoutine(string sceneName)
        {
            var op = SceneManager.UnloadSceneAsync(sceneName);
            if (op == null) yield break;
            yield return op;

            Debug.Log($"[SceneLoader][Info] Unloaded '{sceneName}'.");
            EventBus<SceneUnloadedEvent>.Publish(new SceneUnloadedEvent { SceneName = sceneName });
        }
    }
}
