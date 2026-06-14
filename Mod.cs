using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace NightVehiclesMod
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(NightVehiclesMod)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static ModSettings Settings;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            Settings = new ModSettings(this);
            Settings.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(nameof(NightVehiclesMod), Settings, new ModSettings(this));
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));
            GameManager.instance.localizationManager.AddSource("fr-FR", new LocaleFR(Settings));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            updateSystem.UpdateAt<NightVehicleSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<UIBindings>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<NightVehiclesPersistenceSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (Settings != null)
                Settings.UnregisterInOptionsUI();
        }
    }
}