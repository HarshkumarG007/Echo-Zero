using System;
using UnityEngine;
using Unity.Sentis;

namespace EchoZero.Core.ML
{
    public class SentisModelRunner : ISentisModelRunner, IDisposable
    {
        private Model _runtimeModel;
        private IWorker _engine;
        
        public bool IsModelLoaded => _engine != null;

        public void LoadModel(ModelAsset modelAsset)
        {
            if (modelAsset == null) throw new ArgumentNullException(nameof(modelAsset));
            
            UnloadModel();
            
            _runtimeModel = ModelLoader.Load(modelAsset);
            _engine = WorkerFactory.CreateWorker(BackendType.GPUCompute, _runtimeModel);
            Debug.Log("[SentisModelRunner] Model loaded successfully on GPUCompute backend.");
        }

        public Tensor<float> Execute(Tensor<float> inputTensor)
        {
            if (!IsModelLoaded) throw new InvalidOperationException("No model loaded.");
            if (inputTensor == null) throw new ArgumentNullException(nameof(inputTensor));
            
            _engine.Execute(inputTensor);
            
            var output = _engine.PeekOutput() as Tensor<float>;
            return output;
        }

        public void UnloadModel()
        {
            if (_engine != null)
            {
                _engine.Dispose();
                _engine = null;
            }
        }

        public void Dispose()
        {
            UnloadModel();
        }
    }
}
