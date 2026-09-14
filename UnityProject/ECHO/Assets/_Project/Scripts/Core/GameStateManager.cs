using UnityEngine;
using EchoZero.Core.Events;

namespace EchoZero.Core
{
    public struct GamePausedEvent { public bool IsPaused; }

    /// <summary>
    /// Manages the high-level game state (Paused vs Playing).
    /// TASK: TASK-013
    /// </summary>
    public class GameStateManager
    {
        public bool IsPaused { get; private set; }

        public void SetPause(bool pause)
        {
            if (IsPaused == pause) return;

            IsPaused = pause;
            Time.timeScale = pause ? 0f : 1f;

            EventBus<GamePausedEvent>.Publish(new GamePausedEvent { IsPaused = pause });
        }

        public void TogglePause()
        {
            SetPause(!IsPaused);
        }
    }
}
