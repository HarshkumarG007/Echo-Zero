using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.World;
using EchoZero.Narrative;

namespace EchoZero.Data.Save
{
    // ------------------------------------------------------------------ //
    // Data models
    // ------------------------------------------------------------------ //

    /// <summary>All data persisted to disk for a save slot.</summary>
    public class GameSaveData
    {
        public string SaveVersion        { get; set; } = "1.0";
        public string SessionId          { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public SerializableVector3 PlayerPosition { get; set; } = new();
        public string ActiveSceneName    { get; set; } = "AerieUpperScene";
        public NarrativeStateData NarrativeState { get; set; } = new();
        public List<string> CollectedFragmentIds { get; set; } = new();
        public string ChosenFragmentId   { get; set; }  // null = choice not yet made
    }


    /// <summary>Unity Vector3 serializable via System.Text.Json.</summary>
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static SerializableVector3 From(Vector3 v) => new() { X = v.x, Y = v.y, Z = v.z };
        public Vector3 ToVector3() => new(X, Y, Z);
    }

    // Source-generated JSON serializer context (AOT-safe)
    [JsonSerializable(typeof(GameSaveData))]
    [JsonSerializable(typeof(NarrativeStateData))]
    [JsonSerializable(typeof(SerializableVector3))]
    internal partial class SaveJsonContext : JsonSerializerContext { }

    // ------------------------------------------------------------------ //
    // Service interface
    // ------------------------------------------------------------------ //

    /// <summary>Interface for save/load operations. Allows test substitution.</summary>
    public interface ISaveService
    {
        void Save(GameSaveData data);
        GameSaveData Load();
        bool SaveExists { get; }
    }

    // ------------------------------------------------------------------ //
    // SaveService implementation
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Writes and reads GameSaveData to local disk as plain JSON (ADR-0002, ADR-0006).
    /// No encryption, no checksums — this is a single-player local save file; the only
    /// person who could "tamper" with it is the player on their own machine, which is
    /// not a threat model for this slice. A corrupt or malformed file returns null and
    /// starts a new game rather than crashing.
    ///
    /// Save path: Application.persistentDataPath/save_01.json
    ///
    /// TASK: TASK-003 (stub registered) / TASK-006 (full implementation)
    /// </summary>
    public class SaveService : ISaveService
    {
        private static readonly Unity.Profiling.ProfilerMarker s_SaveMarker =
            new("SaveService.Save");

        private readonly string _savePath;

        public SaveService()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "save_01.json");
        }

        /// <inheritdoc/>
        public bool SaveExists => File.Exists(_savePath);

        /// <inheritdoc/>
        public void Save(GameSaveData data)
        {
            using var _ = s_SaveMarker.Auto();

            try
            {
                var json = JsonSerializer.Serialize(data, SaveJsonContext.Default.GameSaveData);
                File.WriteAllText(_savePath, json, Encoding.UTF8);

                EventBus<SaveCompletedEvent>.Publish(new SaveCompletedEvent { Success = true });
                Debug.Log("[SaveService][Info] Save written successfully.");
            }
            catch (Exception ex)
            {
                EventBus<SaveCompletedEvent>.Publish(new SaveCompletedEvent { Success = false });
                Debug.LogError($"[SaveService][Error] Save failed: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public GameSaveData Load()
        {
            if (!SaveExists)
            {
                Debug.Log("[SaveService][Info] No save found. Starting new game.");
                return null;
            }

            try
            {
                var json = File.ReadAllText(_savePath, Encoding.UTF8);
                var data = JsonSerializer.Deserialize(json, SaveJsonContext.Default.GameSaveData);
                Debug.Log("[SaveService][Info] Save loaded successfully.");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveService][Error] Load failed: {ex.Message}. Starting new game.");
                return null;
            }
        }
    }
}
