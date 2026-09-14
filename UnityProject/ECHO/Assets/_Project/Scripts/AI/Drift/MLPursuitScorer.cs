using System;
using UnityEngine;
using Unity.InferenceEngine;
using EchoZero.Core;
using EchoZero.Core.ML;

namespace EchoZero.AI.Utility
{
    /// <summary>
    /// An ML-driven scorer that evaluates the "Pursue" action.
    /// It uses ISentisModelRunner to predict the player's interception point based on velocity and distance.
    /// If the model is missing or inference fails, it falls back to a simple distance-based heuristic.
    /// TASK: TASK-023
    /// </summary>
    public class MLPursuitScorer : UtilityScorer
    {
        private readonly Func<float> _distanceProvider;
        private readonly Func<Vector3> _velocityProvider;
        private readonly float _detectionRadius;

        public MLPursuitScorer(Func<float> distanceProvider, Func<Vector3> velocityProvider, float detectionRadius)
        {
            _distanceProvider = distanceProvider;
            _velocityProvider = velocityProvider;
            _detectionRadius = detectionRadius;
        }

        public override float Score()
        {
            float distance = _distanceProvider();
            Vector3 velocity = _velocityProvider();

            // Baseline fallback heuristic: 1.0 if inside detection radius, else 0.
            float baselineScore = distance < _detectionRadius ? 1f : 0f;

            if (ServiceLocator.TryGet<ISentisModelRunner>(out var runner) && runner.IsModelLoaded)
            {
                try
                {
                    // Prepare input tensor: [Distance, VelocityX, VelocityZ]
                    using var inputTensor = new Tensor<float>(new TensorShape(1, 3), new float[] { distance, velocity.x, velocity.z });
                    
                    // Execute inference
                    using var outputTensor = runner.Execute(inputTensor);
                    
                    // Assume model outputs a score multiplier for pursuit between 0 and 2.
                    // For example, if player is moving away fast, score might be higher to intercept.
                    float mlScore = outputTensor[0];
                    return Mathf.Clamp(mlScore, 0f, 2f);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[MLPursuitScorer] Inference failed, falling back to baseline. Error: {e.Message}");
                    return baselineScore;
                }
            }

            // Fallback if no model is loaded
            return baselineScore;
        }
    }
}
