using Game.Policies;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Unity.Entities;

namespace NightVehiclesMod
{
    public static class VehicleCountCalculator
    {
        public static float CalculateAdjustmentFromVehicleCount(
            int vehicleCount,
            float originalInterval,
            float duration,
            DynamicBuffer<RouteModifierData> modifierDatas,
            PolicySliderData sliderData)
        {
            float targetInterval = TransportLineSystem.CalculateVehicleInterval(duration, vehicleCount);
            RouteModifier modifier = default;

            for (int i = 0; i < modifierDatas.Length; i++)
            {
                var item = modifierDatas[i];
                if (item.m_Type == RouteModifierType.VehicleInterval)
                {
                    if (item.m_Mode == ModifierValueMode.Absolute)
                        modifier.m_Delta.x = targetInterval - originalInterval;
                    else
                        modifier.m_Delta.y = (-originalInterval + targetInterval) / originalInterval;

                    float modifierDelta = RouteModifierInitializeSystem.RouteModifierRefreshData
                        .GetDeltaFromModifier(modifier, item);
                    return RouteModifierInitializeSystem.RouteModifierRefreshData
                        .GetPolicyAdjustmentFromModifierDelta(item, modifierDelta, sliderData);
                }
            }
            return -1f;
        }
    }
}