using Unity.Sentis;

namespace EchoZero.Core.ML
{
    public interface ISentisModelRunner
    {
        bool IsModelLoaded { get; }
        void LoadModel(ModelAsset modelAsset);
        Tensor<float> Execute(Tensor<float> inputTensor);
        void UnloadModel();
    }
}
