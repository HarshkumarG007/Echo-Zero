using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.UI;

namespace EchoZero.Tests.EditMode
{
    public class PauseMenuTests
    {
        private GameStateManager _gameStateManager;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _gameStateManager = new GameStateManager();
            ServiceLocator.Register<GameStateManager>(_gameStateManager);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            EventBus<GamePausedEvent>.Clear();
            Time.timeScale = 1f; // Ensure time scale is reset
        }

        [Test]
        public void GameStateManager_TogglePause_SetsTimeScaleAndFiresEvent()
        {
            bool eventFired = false;
            EventBus<GamePausedEvent>.Subscribe(e =>
            {
                eventFired = true;
                Assert.IsTrue(e.IsPaused);
            });

            _gameStateManager.TogglePause();

            Assert.IsTrue(eventFired);
            Assert.IsTrue(_gameStateManager.IsPaused);
            Assert.AreEqual(0f, Time.timeScale);

            // Toggle again
            _gameStateManager.TogglePause();
            Assert.IsFalse(_gameStateManager.IsPaused);
            Assert.AreEqual(1f, Time.timeScale);
        }

        [Test]
        public void PauseMenuUI_SaveClicked_CallsSaveService()
        {
            var saveServiceMock = Substitute.For<ISaveService>();
            ServiceLocator.Register<ISaveService>(saveServiceMock);

            var go = new GameObject();
            var ui = go.AddComponent<PauseMenuUI>();

            // Simulate the button click via reflection to bypass UI Toolkit VisualTree setup in EditMode
            var onSaveMethod = typeof(PauseMenuUI).GetMethod("OnSaveClicked", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            onSaveMethod?.Invoke(ui, null);

            saveServiceMock.Received(1).SaveGame();

            Object.DestroyImmediate(go);
        }
    }
}
