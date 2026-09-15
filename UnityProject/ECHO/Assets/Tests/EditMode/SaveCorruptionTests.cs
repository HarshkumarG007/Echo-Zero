using NUnit.Framework;
using System.IO;
using UnityEngine;
using EchoZero.Data.Save;

namespace EchoZero.Tests.EditMode
{
    /// <summary>
    /// Tests for SaveService — plain JSON persistence, per ADR-0002 and ADR-0006.
    /// No checksum behaviour is tested here; checksums were removed in ADR-0006.
    /// </summary>
    public class SaveCorruptionTests
    {
        private SaveService _saveService;
        private string _savePath;

        [SetUp]
        public void SetUp()
        {
            _saveService = new SaveService();
            _savePath = Path.Combine(Application.persistentDataPath, "save_01.json");

            // Clean up any existing saves before each test
            if (File.Exists(_savePath)) File.Delete(_savePath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
        }

        [Test]
        public void Load_WhenValidJson_ReturnsValidData()
        {
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            var loadedData = _saveService.Load();

            Assert.IsNotNull(loadedData);
            Assert.AreEqual("TestScene", loadedData.ActiveSceneName);
        }

        [Test]
        public void Load_WhenJsonHandEdited_LoadsSuccessfully()
        {
            // ADR-0006: hand-editing a local save file is not a threat — load should succeed.
            var dataToSave = new GameSaveData { ActiveSceneName = "TestScene" };
            _saveService.Save(dataToSave);

            // Simulate a player opening save_01.json and changing data
            string json = File.ReadAllText(_savePath);
            string editedJson = json.Replace("TestScene", "EditedScene");
            File.WriteAllText(_savePath, editedJson);

            // Should load the player's edited data fine — no checksum to block it
            var loadedData = _saveService.Load();

            Assert.IsNotNull(loadedData, "SaveService should load a hand-edited save file successfully.");
            Assert.AreEqual("EditedScene", loadedData.ActiveSceneName);
        }

        [Test]
        public void Load_WhenNoSaveExists_ReturnsNull()
        {
            // Ensure file does not exist
            if (File.Exists(_savePath)) File.Delete(_savePath);

            var loadedData = _saveService.Load();

            Assert.IsNull(loadedData, "SaveService should return null when no save file exists.");
        }

        [Test]
        public void Load_WhenJsonIsInvalid_ReturnsNull()
        {
            // Write garbage to the save file
            File.WriteAllText(_savePath, "{ invalid_json: ");

            // Defensive parsing: malformed JSON should return null, not throw
            var loadedData = _saveService.Load();

            Assert.IsNull(loadedData, "SaveService should gracefully handle malformed JSON and return null.");
        }

        [Test]
        public void SaveExists_ReturnsFalse_WhenNoFileOnDisk()
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
            Assert.IsFalse(_saveService.SaveExists);
        }

        [Test]
        public void SaveExists_ReturnsTrue_AfterSave()
        {
            _saveService.Save(new GameSaveData());
            Assert.IsTrue(_saveService.SaveExists);
        }
    }
}
