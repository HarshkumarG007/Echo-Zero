using System;
using NUnit.Framework;
using EchoZero.Core.ML;
using Unity.InferenceEngine;

namespace EchoZero.Tests.EditMode
{
    public class SentisModelRunnerTests
    {
        private SentisModelRunner _runner;

        [SetUp]
        public void Setup()
        {
            _runner = new SentisModelRunner();
        }

        [TearDown]
        public void TearDown()
        {
            _runner?.Dispose();
        }

        [Test]
        public void LoadModel_NullAsset_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _runner.LoadModel(null));
        }

        [Test]
        public void Execute_NoModelLoaded_ThrowsInvalidOperationException()
        {
            var tensor = new Tensor<float>(new TensorShape(1, 1), new float[] { 0f });
            Assert.Throws<InvalidOperationException>(() => _runner.Execute(tensor));
            tensor.Dispose();
        }

        [Test]
        public void IsModelLoaded_InitiallyFalse()
        {
            Assert.IsFalse(_runner.IsModelLoaded);
        }

        [Test]
        public void UnloadModel_DoesNotThrowWhenEmpty()
        {
            Assert.DoesNotThrow(() => _runner.UnloadModel());
        }
    }
}
