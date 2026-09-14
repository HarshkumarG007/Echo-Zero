using System.Collections;
using UnityEngine;

namespace EchoZero.Gameplay.VFX
{
    /// <summary>
    /// Animates a material float property over time via coroutines.
    /// Used for RECALL material dissolves and Memory Formation crystalline emission.
    /// TASK: TASK-018
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialPropertyFader : MonoBehaviour
    {
        [Tooltip("The exact name of the exposed shader property (e.g. '_EmissionIntensity' or '_Dissolve').")]
        [SerializeField] private string _propertyName;

        private Renderer _renderer;
        private Material _instancedMaterial;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null && _renderer.material != null)
            {
                // Create an instance so we don't modify the shared asset
                _instancedMaterial = _renderer.material;
            }
        }

        public void FadeTo(float targetValue, float duration)
        {
            if (_instancedMaterial == null || string.IsNullOrEmpty(_propertyName)) return;
            if (!_instancedMaterial.HasProperty(_propertyName))
            {
                Debug.LogWarning($"[MaterialPropertyFader] Property {_propertyName} not found on material {_instancedMaterial.name}");
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeRoutine(targetValue, duration));
        }

        private IEnumerator FadeRoutine(float targetValue, float duration)
        {
            float startValue = _instancedMaterial.GetFloat(_propertyName);
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float current = Mathf.Lerp(startValue, targetValue, time / duration);
                _instancedMaterial.SetFloat(_propertyName, current);
                yield return null;
            }

            _instancedMaterial.SetFloat(_propertyName, targetValue);
            _fadeCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_instancedMaterial != null)
            {
                Destroy(_instancedMaterial);
            }
        }
    }
}
