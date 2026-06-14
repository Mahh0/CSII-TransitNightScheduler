using Game;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.UI.InGame;
using Unity.Entities;

namespace NightVehiclesMod
{
    public partial class NightVehicleSystem : GameSystemBase
    {
        private TimeSystem _timeSystem;
        private PoliciesUISystem _policiesUISystem;
        private PrefabSystem _prefabSystem;
        private EntityQuery _configQuery;
        private EntityQuery _transportLineQuery;
        private Entity _vehicleCountPolicy;
        private bool _isNight;
        private bool _wasNight;
        private bool _policiesLoaded;
        public Entity VehicleCountPolicy => _vehicleCountPolicy;

        protected override void OnCreate()
        {
            base.OnCreate();
            _timeSystem = World.GetOrCreateSystemManaged<TimeSystem>();
            _policiesUISystem = World.GetOrCreateSystemManaged<PoliciesUISystem>();
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            _configQuery = GetEntityQuery(ComponentType.ReadOnly<UITransportConfigurationData>());
            _transportLineQuery = GetEntityQuery(ComponentType.ReadOnly<TransportLine>());
            Mod.log.Info("NightVehicleSystem created");
        }

        private void TryLoadPolicies()
        {
            if (_policiesLoaded || _configQuery.IsEmptyIgnoreFilter) return;
            var prefab = _prefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(_configQuery);
            _vehicleCountPolicy = _prefabSystem.GetEntity(prefab.m_VehicleCountPolicy);
            _policiesLoaded = true;
            Mod.log.Info($"Policies loaded: vehicleCount={_vehicleCountPolicy.Index}");
        }

        private string GetStableKey(Entity entity)
        {
            if (!EntityManager.HasComponent<RouteNumber>(entity)) return null;
            if (!EntityManager.HasComponent<PrefabRef>(entity)) return null;
            var routeNumber = EntityManager.GetComponentData<RouteNumber>(entity);
            var prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);
            if (!EntityManager.HasComponent<TransportLineData>(prefabRef.m_Prefab)) return null;
            var transportLineData = EntityManager.GetComponentData<TransportLineData>(prefabRef.m_Prefab);
            return $"{transportLineData.m_TransportType}_{routeNumber.m_Number}";
        }

        protected override void OnUpdate()
        {
            if (Mod.Settings != null && !Mod.Settings.EnableNightMode) return;

            TryLoadPolicies();
            if (!_policiesLoaded) return;

            float timeOfDay = _timeSystem.normalizedTime;
            float nightStart = Mod.Settings != null ? Mod.Settings.NightStartHour / 24f : 0.875f;
            float nightEnd = Mod.Settings != null ? Mod.Settings.NightEndHour / 24f : 0.25f;

            _isNight = nightStart > nightEnd
                ? timeOfDay > nightStart || timeOfDay < nightEnd
                : timeOfDay > nightStart && timeOfDay < nightEnd;

            if (_isNight == _wasNight) return;
            _wasNight = _isNight;

            Mod.log.Info(_isNight ? "Switching to NIGHT mode" : "Switching to DAY mode");

            var policySliderData = EntityManager.GetComponentData<PolicySliderData>(_vehicleCountPolicy);
            var modifierDatas = EntityManager.GetBuffer<RouteModifierData>(_vehicleCountPolicy, true);

            var entities = _transportLineQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            Mod.log.Info($"Found {entities.Length} transport lines");

            foreach (var entity in entities)
            {
                string lineKey = GetStableKey(entity);
                if (lineKey == null) continue;
                if (!NightVehicleSettings.LineNightVehicles.ContainsKey(lineKey)) continue;

                if (!EntityManager.HasComponent<PrefabRef>(entity)) continue;
                var prefabRef = EntityManager.GetComponentData<PrefabRef>(entity);
                if (!EntityManager.HasComponent<TransportLineData>(prefabRef.m_Prefab)) continue;

                var transportLineData = EntityManager.GetComponentData<TransportLineData>(prefabRef.m_Prefab);
                var transportLine = EntityManager.GetComponentData<TransportLine>(entity);

                float currentAdjustment = 0f;
                bool policyFound = false;
                if (EntityManager.HasBuffer<Game.Policies.Policy>(entity))
                {
                    var policies = EntityManager.GetBuffer<Game.Policies.Policy>(entity, true);
                    for (int i = 0; i < policies.Length; i++)
                    {
                        if (policies[i].m_Policy == _vehicleCountPolicy)
                        {
                            currentAdjustment = policies[i].m_Adjustment;
                            policyFound = true;
                            break;
                        }
                    }
                }

                Mod.log.Info($"Line {lineKey}: interval={transportLine.m_VehicleInterval} currentAdjustment={currentAdjustment} policyFound={policyFound}");

                float targetAdjustment;

                if (_isNight)
                {
                    if (!NightVehicleSettings.LineDayAdjustments.ContainsKey(lineKey))
                    {
                        NightVehicleSettings.LineDayAdjustments[lineKey] = currentAdjustment;
                        Mod.log.Info($"Line {lineKey}: saved day adjustment={currentAdjustment}");
                        World.GetOrCreateSystemManaged<NightVehiclesPersistenceSystem>().Save();
                    }

                    int targetCount = NightVehicleSettings.LineNightVehicles[lineKey];
                    float estimatedDuration = transportLine.m_VehicleInterval * targetCount;

                    targetAdjustment = VehicleCountCalculator.CalculateAdjustmentFromVehicleCount(
                        targetCount,
                        transportLineData.m_DefaultVehicleInterval,
                        estimatedDuration,
                        modifierDatas,
                        policySliderData);
                }
                else
                {
                    targetAdjustment = NightVehicleSettings.LineDayAdjustments.TryGetValue(lineKey, out float dayAdj)
                        ? dayAdj : currentAdjustment;
                }

                Mod.log.Info($"Line {lineKey}: applying adjustment={targetAdjustment}");
                _policiesUISystem.SetPolicy(entity, _vehicleCountPolicy, true, targetAdjustment);
            }
            entities.Dispose();
        }
    }
}