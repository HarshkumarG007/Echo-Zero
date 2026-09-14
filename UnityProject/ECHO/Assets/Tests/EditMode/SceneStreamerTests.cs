using NUnit.Framework;
using UnityEngine;
using EchoZero.Core.Scenes;

namespace EchoZero.Tests.EditMode
{
    public class SceneStreamerTests
    {
        [Test]
        public void SceneStreamer_RequiresCollider_AndSetsItToTrigger()
        {
            var go = new GameObject();
            // In Awake, it gets the collider and sets isTrigger = true.
            // Since it's EditMode, Awake might not run automatically on AddComponent, 
            // but RequireComponent will add a BoxCollider by default if not present in Editor, 
            // or we add it manually and test Awake behavior via reflection.

            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = false;

            var streamer = go.AddComponent<SceneStreamer>();

            // Simulate Awake
            var awakeMethod = typeof(SceneStreamer).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            awakeMethod?.Invoke(streamer, null);

            Assert.IsTrue(col.isTrigger);

            Object.DestroyImmediate(go);
        }
    }
}
