using System.Diagnostics;
using System.Text;
using UnityEngine;
using Unity.InferenceEngine;
using EchoZero.Core.ML;

namespace EchoZero.Tools.Benchmarking
{
    /// <summary>
    /// Executes headless-compatible benchmarks for ML inference to measure performance
    /// against the Phase 6 baseline (as dictated in AGENTS.md §10 Performance Budgets).
    /// </summary>
    public class PerformanceBenchmark : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ModelAsset _modelAsset;
        [SerializeField] private int _warmupIterations = 100;
        [SerializeField] private int _benchmarkIterations = 1000;

        private void Start()
        {
            if (_modelAsset == null)
            {
                UnityEngine.Debug.LogWarning("[Benchmark] No ModelAsset assigned. Skipping ML benchmark.");
                return;
            }

            RunBenchmark();
        }

        [ContextMenu("Run Benchmark")]
        public void RunBenchmark()
        {
            UnityEngine.Debug.Log($"[Benchmark] Starting benchmark with {_benchmarkIterations} iterations...");

            using var runner = new SentisModelRunner();
            runner.LoadModel(_modelAsset);

            // Prepare dummy input tensor for MLPursuitScorer (shape: 1x3)
            using var inputTensor = new Tensor<float>(new TensorShape(1, 3), new float[] { 15f, 5f, -2f });

            // Warmup
            for (int i = 0; i < _warmupIterations; i++)
            {
                runner.Execute(inputTensor);
            }

            // Benchmark
            long minTicks = long.MaxValue;
            long maxTicks = long.MinValue;
            long totalTicks = 0;

            var stopwatch = new Stopwatch();

            for (int i = 0; i < _benchmarkIterations; i++)
            {
                stopwatch.Restart();
                
                runner.Execute(inputTensor);
                
                stopwatch.Stop();
                long ticks = stopwatch.ElapsedTicks;
                
                if (ticks < minTicks) minTicks = ticks;
                if (ticks > maxTicks) maxTicks = ticks;
                totalTicks += ticks;
            }

            // Report
            double msPerTick = 1000.0 / Stopwatch.Frequency;
            double minMs = minTicks * msPerTick;
            double maxMs = maxTicks * msPerTick;
            double avgMs = (totalTicks / (double)_benchmarkIterations) * msPerTick;

            var sb = new StringBuilder();
            sb.AppendLine("=== ML Inference Benchmark Results ===");
            sb.AppendLine($"Iterations: {_benchmarkIterations}");
            sb.AppendLine($"Backend: GPUCompute (Default)");
            sb.AppendLine($"Avg Time: {avgMs:F4} ms");
            sb.AppendLine($"Min Time: {minMs:F4} ms");
            sb.AppendLine($"Max Time: {maxMs:F4} ms");
            sb.AppendLine("=======================================");

            UnityEngine.Debug.Log(sb.ToString());
            
            // Validate budget
            if (avgMs > 1.0) // Budget is 8ms for the entire frame; ML shouldn't take >1ms
            {
                UnityEngine.Debug.LogWarning($"[Benchmark] ML inference exceeds 1ms budget limit ({avgMs:F4} ms)!");
            }
            else
            {
                UnityEngine.Debug.Log($"[Benchmark] ML inference is within performance budget.");
            }
        }
    }
}
