using System;
using System.Collections.Generic;
using System.IO;
using Colossal.Json;
using Game;
using Game.SceneFlow;

namespace NightVehiclesMod
{
    public partial class NightVehiclesPersistenceSystem : GameSystemBase
    {
        private string _savePath;

        private static string ModsDataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "..", "LocalLow", "Colossal Order", "Cities Skylines II", "ModsData", "NightVehiclesMod");

        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.log.Info("PersistenceSystem created");
        }

        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, Game.GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode != Game.GameMode.Game) return;

            _savePath = GetSavePath();
            Mod.log.Info($"PersistenceSystem: save path = {_savePath}");
            Load();
        }

        private string GetSavePath()
        {
            try
            {
                var meta = GameManager.instance.settings.userState.lastSaveGameMetadata;
                string saveId = meta?.name ?? "unknown";
                saveId = string.Join("_", saveId.Split(Path.GetInvalidFileNameChars()));
                return Path.Combine(ModsDataPath, $"{saveId}.json");
            }
            catch (Exception e)
            {
                Mod.log.Error($"PersistenceSystem: failed to get save path: {e.Message}");
                return Path.Combine(ModsDataPath, "default.json");
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_savePath)) return;
            try
            {
                Directory.CreateDirectory(ModsDataPath);
                var data = new SaveData
                {
                    lineNightVehicles = new Dictionary<string, int>(NightVehicleSettings.LineNightVehicles),
                    lineDayAdjustments = new Dictionary<string, float>(NightVehicleSettings.LineDayAdjustments)
                };
                string json = JSON.Dump(data);
                File.WriteAllText(_savePath, json);
                Mod.log.Info($"PersistenceSystem: saved {data.lineNightVehicles.Count} lines");
            }
            catch (Exception e)
            {
                Mod.log.Error($"PersistenceSystem: save failed: {e.Message}");
            }
        }
        private void Load()
        {
            if (string.IsNullOrEmpty(_savePath) || !File.Exists(_savePath)) return;
            try
            {
                string json = File.ReadAllText(_savePath);
                var data = JSON.MakeInto<SaveData>(JSON.Load(json));
                if (data?.lineNightVehicles != null)
                    NightVehicleSettings.LineNightVehicles = data.lineNightVehicles;
                if (data?.lineDayAdjustments != null)
                    NightVehicleSettings.LineDayAdjustments = data.lineDayAdjustments;
                Mod.log.Info($"PersistenceSystem: loaded {NightVehicleSettings.LineNightVehicles.Count} lines");
            }
            catch (Exception e)
            {
                Mod.log.Error($"PersistenceSystem: load failed: {e.Message}");
            }
        }

        protected override void OnUpdate() { }

        [Serializable]
        public class SaveData
        {
            public Dictionary<string, int> lineNightVehicles;
            public Dictionary<string, float> lineDayAdjustments;
        }
    }
}