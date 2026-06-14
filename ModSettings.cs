using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace NightVehiclesMod
{
    [FileLocation(nameof(NightVehiclesMod))]
    [SettingsUIShowGroupName("Schedule")]
    public class ModSettings : ModSetting
    {
        public ModSettings(IMod mod) : base(mod) { }

        [SettingsUISection("Schedule")]
        public bool EnableNightMode { get; set; } = true;
        
        [SettingsUISection("Schedule")]
        [SettingsUISlider(min = 0, max = 23, step = 1, unit = "integer")]
        public int NightStartHour { get; set; } = 21;

        [SettingsUISection("Schedule")]
        [SettingsUISlider(min = 0, max = 23, step = 1, unit = "integer")]
        public int NightEndHour { get; set; } = 6;

        public override void SetDefaults()
        {
            NightStartHour = 21;
            NightEndHour = 6;
            EnableNightMode = true;
        }
    }
}