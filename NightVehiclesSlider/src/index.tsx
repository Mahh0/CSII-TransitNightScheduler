import { ModRegistrar } from "cs2/modding";
import { NightVehiclesSection } from "mods/night-vehicles-section";

const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.extend(
        'game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx',
        'selectedInfoSectionComponents',
        (components: any) => {
            const original = components["Game.UI.InGame.VehicleCountSection"];
            return {
                ...components,
                "Game.UI.InGame.VehicleCountSection": NightVehiclesSection(original)
            };
        }
    );

    console.log("NightVehiclesMod UI registered.");
}

export default register;
