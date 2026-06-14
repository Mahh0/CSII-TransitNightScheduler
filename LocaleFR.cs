using System.Collections.Generic;
using Colossal;

namespace NightVehiclesMod
{
    public class LocaleFR : IDictionarySource
    {
        private readonly ModSettings _settings;
        public LocaleFR(ModSettings settings) => _settings = settings;

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { _settings.GetSettingsLocaleID(), "Night Vehicles" },
                { _settings.GetOptionTabLocaleID("Schedule"), "Calendrier" },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.EnableNightMode)), "Activer le mode nuit" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.EnableNightMode)), "Active ou désactive la réduction de véhicules nocturnes sur toutes les lignes." },
                { _settings.GetOptionGroupLocaleID("Schedule"), "Horaires nocturnes" },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.NightStartHour)), "Début de la nuit (heure)" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.NightStartHour)), "Heure de début du mode nuit. Pour une nuit qui chevauche minuit (ex: 21h à 6h), cette valeur doit être supérieure à l'heure de fin." },
                { _settings.GetOptionLabelLocaleID(nameof(ModSettings.NightEndHour)), "Fin de la nuit (heure)" },
                { _settings.GetOptionDescLocaleID(nameof(ModSettings.NightEndHour)), "Heure de fin du mode nuit. Pour une nuit qui chevauche minuit (ex: 21h à 6h), cette valeur doit être inférieure à l'heure de début." },
            };
        }

        public void Unload() { }
    }
}