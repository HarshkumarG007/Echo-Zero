using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.ML;
using EchoZero.AI.Utility;
using NSubstitute;
using Unity.InferenceEngine;

namespace EchoZero.Tests.EditMode
{
    public class MLPursuitScorerTests
    {
        private ISentisModelRunner _mockRunner;

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Clear();
            _mockRunner = Substitute.For<ISentisModelRunner>();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
        }

        [Test]
        public void Score_WhenModelNotLoaded_UsesFallbackHeuristic()
        {
            // Arrange
            _mockRunner.IsModelLoaded.Returns(false);
            ServiceLocator.Register<ISentisModelRunner>(_mockRunner);

            var scorer = new MLPursuitScorer(
                distanceProvider: () => 10f,
                velocityProvider: () => Vector3.zero,
                detectionRadius: 15f
            );

            // Act
            float score = scorer.Score();

            // Assert - falls back to 1.0f because distance (10) < detectionRadius (15)
            Assert.AreEqual(1f, score);
        }

        [Test]
        public void Score_WhenNoServiceRegistered_UsesFallbackHeuristic()
        {
            // Arrange - do NOT register ISentisModelRunner
            var scorer = new MLPursuitScorer(
                distanceProvider: () => 20f,
                velocityProvider: () => Vector3.zero,
                detectionRadius: 15f
            );

            // Act
            float score = scorer.Score();

            // Assert - falls back to 0.0f because distance (20) > detectionRadius (15)
            Assert.AreEqual(0f, score);
        }

        [Test]
        public void Score_WhenModelLoaded_ReturnsMlScore()
        {
            // Arrange
            _mockRunner.IsModelLoaded.Returns(true);
            
            // Mock the Execute call to return a tensor with value 1.5f
            var mockOutput = new Tensor<float>(new TensorShape(1, 1), new float[] { 1.5f });
            _mockRunner.Execute(Arg.Any<Tensor<float>>()).Returns(mockOutput);
            
            ServiceLocator.Register<ISentisModelRunner>(_mockRunner);

            var scorer = new MLPursuitScorer(
                distanceProvider: () => 5f,
                velocityProvider: () => new Vector3(1, 0, 1),
                detectionRadius: 15f
            );

            // Act
            float score = scorer.Score();

            // Assert
            Assert.AreEqual(1.5f, score);
            
            // Cleanup the mock tensor we created manually
            mockOutput.Dispose();
        }

        [Test]
        public void Score_WhenInferenceThrows_UsesFallbackHeuristic()
        {
            // Arrange
            _mockRunner.IsModelLoaded.Returns(true);
            _mockRunner.Execute(Arg.Any<Tensor<float>>()).Returns(x => throw new System.Exception("Simulated inference crash"));
            ServiceLocator.Register<ISentisModelRunner>(_mockRunner);

            var scorer = new MLPursuitScorer(
                distanceProvider: () => 5f, // Inside radius = 1.0f fallback score
                velocityProvider: () => Vector3.zero,
                detectionRadius: 15f
            );

            // Act
            float score = scorer.Score();

            // Assert
            Assert.AreEqual(1f, score);
        }
    }
}
