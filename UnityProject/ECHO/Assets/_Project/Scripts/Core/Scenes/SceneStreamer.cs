using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace EchoZero.Core.Scenes
{
    /// <summary>
    /// Additively loads and unloads scenes based on trigger volumes.
    /// TASK: TASK-012
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SceneStreamer : MonoBehaviour
    {
        [Tooltip("The exact name of the scene to load when entering this trigger.")]
        [SerializeField] private string _sceneToLoad;
        
        [Tooltip("Scenes to unload when entering this trigger (optional).")]
        [SerializeField] private List<string> _scenesToUnload = new();

        private bool _isLoaded = false;
        private Coroutine _loadingCoroutine;

        private void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Usually we'd check for a Player tag or component here.
            if (other.CompareTag("Player") || other.GetComponent<EchoZero.Gameplay.PlayerController>() != null)
            {
                if (!_isLoaded && !string.IsNullOrEmpty(_sceneToLoad))
                {
                    _loadingCoroutine = StartCoroutine(LoadSceneAsync(_sceneToLoad));
                }

                foreach (var sceneName in _scenesToUnload)
                {
                    if (IsSceneLoaded(sceneName))
                    {
                        StartCoroutine(UnloadSceneAsync(sceneName));
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // For now, we rely on entering adjacent triggers to unload, rather than strict exit,
            // to prevent popping if the player steps back and forth on a threshold.
            // But we could implement a buffer zone logic here if needed.
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            _isLoaded = true;
            if (!IsSceneLoaded(sceneName))
            {
                var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                Debug.Log($"[SceneStreamer] Loaded scene: {sceneName}");
            }
            _loadingCoroutine = null;
        }

        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            var asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
            while (asyncUnload != null && !asyncUnload.isDone)
            {
                yield return null;
            }
            Debug.Log($"[SceneStreamer] Unloaded scene: {sceneName}");
        }

        private bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
