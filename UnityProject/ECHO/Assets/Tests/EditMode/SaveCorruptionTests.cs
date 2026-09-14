using NUnit.Framework;
using System.IO;
using UnityEngine;
using EchoZero.Data.Save;

namespace EchoZero.Tests.EditMode
{
    public class SaveCorruptionTests
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

            // Clean up any existing saves before testing
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
        public void Load_WhenChecksumMatches_ReturnsValidData()
        {
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            var loadedData = _saveService.Load();

            Assert.IsNotNull(loadedData);
            Assert.AreEqual("TestScene", loadedData.ActiveSceneName);
        }

        [Test]
        public void Load_WhenJsonTampered_ReturnsNullAndResets()
        {
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            // Simulate a player opening save_01.json and changing data
            string json = File.ReadAllText(_savePath);
            string tamperedJson = json.Replace("TestScene", "HackedScene");
            File.WriteAllText(_savePath, tamperedJson);

            // Attempt to load. Checksum should fail, returning null (fallback to new game)
            var loadedData = _saveService.Load();

            Assert.IsNull(loadedData, "SaveService should return null if the checksum does not match the JSON payload.");
        }

        [Test]
        public void Load_WhenMissingChecksum_ReturnsNull()
        {
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            // Delete checksum file
            File.Delete(_checksumPath);

            var loadedData = _saveService.Load();

            Assert.IsNull(loadedData, "SaveService should return null if the checksum file is missing.");
        }
        
        [Test]
        public void Load_WhenJsonIsInvalid_ReturnsNull()
        {
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            // Write garbage to the save file
            File.WriteAllText(_savePath, "{ invalid_json: ");

            // Attempt to load. Json parsing should throw/fail, returning null
            var loadedData = _saveService.Load();

            Assert.IsNull(loadedData, "SaveService should gracefully handle malformed JSON and return null.");
        }
    }
}
