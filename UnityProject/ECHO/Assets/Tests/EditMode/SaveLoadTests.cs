using NUnit.Framework;
using System.IO;
using UnityEngine;
using EchoZero.Data.Save;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

namespace EchoZero.Tests.EditMode
{
    public class SaveLoadTests
    {
        private SaveService _saveService;
        private string _savePath;
        private string _checksumPath;

        [SetUp]
        public void SetUp()
        {
            _saveService = new SaveService();
            _savePath = Path.Combine(Application.persistentDataPath, "save_01.json");
            _checksumPath = Path.Combine(Application.persistentDataPath, "save_01.checksum");
            
            // Clean up any existing save
            if (File.Exists(_savePath)) File.Delete(_savePath);
            if (File.Exists(_checksumPath)) File.Delete(_checksumPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
            if (File.Exists(_checksumPath)) File.Delete(_checksumPath);
        }

        [Test]
        public void SaveLoad_RoundTrip_ReturnsIdenticalData()
        {
            var data = new GameSaveData
            {
                SaveVersion = "1.0",
                SessionId = "test_session",
                ActiveSceneName = "TestScene",
                PlayerPosition = new SerializableVector3 { X = 1, Y = 2, Z = 3 },
                CollectedFragmentIds = new System.Collections.Generic.List<string> { "frag1", "frag2" }
            };

            LogAssert.Expect(LogType.Log, new Regex(@"\[SaveService\]\[Info\] Save written successfully\."));
            _saveService.Save(data);

            Assert.IsTrue(_saveService.SaveExists);
            
            LogAssert.Expect(LogType.Log, new Regex(@"\[SaveService\]\[Info\] Save loaded successfully\."));
            var loadedData = _saveService.Load();
            
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(data.SessionId, loadedData.SessionId);
            Assert.AreEqual(data.ActiveSceneName, loadedData.ActiveSceneName);
            Assert.AreEqual(data.PlayerPosition.X, loadedData.PlayerPosition.X);
            Assert.AreEqual(data.CollectedFragmentIds.Count, loadedData.CollectedFragmentIds.Count);
            Assert.AreEqual(data.CollectedFragmentIds[0], loadedData.CollectedFragmentIds[0]);
        }

        [Test]
        public void Load_WithCorruptedChecksum_ReturnsNull()
        {
            var data = new GameSaveData { SessionId = "test" };
            LogAssert.Expect(LogType.Log, new Regex(@"\[SaveService\]\[Info\] Save written successfully\."));
            _saveService.Save(data);

            // Corrupt the checksum
            File.WriteAllText(_checksumPath, "invalid_checksum_here");

            LogAssert.Expect(LogType.Warning, "[SaveService][Warning] Save file checksum mismatch. File may be corrupted or tampered. Starting new game.");
            var loadedData = _saveService.Load();
            
            Assert.IsNull(loadedData);
        }

        [Test]
        public void Load_WithMalformedJson_ReturnsNull()
        {
            var data = new GameSaveData { SessionId = "test" };
            LogAssert.Expect(LogType.Log, new Regex(@"\[SaveService\]\[Info\] Save written successfully\."));
            _saveService.Save(data);

            // Corrupt the JSON
            File.WriteAllText(_savePath, "{ invalid json }");
            
            // Need to update checksum to match the malformed JSON so it doesn't fail checksum validation first
            var bytes = System.Text.Encoding.UTF8.GetBytes("{ invalid json }");
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            var sb = new System.Text.StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            File.WriteAllText(_checksumPath, sb.ToString());

            // Expect error log
            LogAssert.Expect(LogType.Error, new Regex(@"\[SaveService\]\[Error\] Load failed.*"));
            
            var loadedData = _saveService.Load();
            
            Assert.IsNull(loadedData);
        }
    }
}
