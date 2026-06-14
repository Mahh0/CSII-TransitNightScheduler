using System.Collections.Generic;
using Colossal;

namespace NightVehiclesMod
{
    public class LocaleEN : IDictionarySource
    {
        private readonly ModSettings _settings;
        public LocaleEN(ModSettings settings) => _settings = settings;

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), "Night Vehicles" },
                { _settings.GetOptionTabLocaleID("Schedule"), "Schedule" },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.EnableNightMode)), "Enable night mode" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.EnableNightMode)), "Enable or disable night vehicle reduction for all lines." },
                { _settings.GetOptionGroupLocaleID("Schedule"), "Night Schedule" },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.NightStartHour)), "Night start (hour)" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.NightStartHour)), "Hour when night mode starts. For a night spanning midnight (e.g. 21h to 6h), set this higher than the end hour." },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.NightEndHour)), "Night end (hour)" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.NightEndHour)), "Hour when night mode ends. For a night spanning midnight (e.g. 21h to 6h), set this lower than the start hour." },
            };
        }

        public void Unload() { }
    }
}