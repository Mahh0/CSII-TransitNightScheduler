using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Routes;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;

namespace NightVehiclesMod
{
    public partial class UIBindings : UISystemBase
    {
        private const string kGroup = "nightVehicles";
        private ValueBinding<int> _selectedLineBinding;
        private ValueBinding<int> _nightVehicleCountBinding;
        private ValueBinding<int> _scheduleBinding;
        private ValueBinding<int> _maxVehiclesBinding;
        private EntityQuery _transportLineQuery;
        private SelectedInfoUISystem _selectedInfoSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _selectedLineBinding = new ValueBinding<int>(kGroup, "selectedLineIndex", -1);
            _nightVehicleCountBinding = new ValueBinding<int>(kGroup, "nightVehicleCount", 1);
            _scheduleBinding = new ValueBinding<int>(kGroup, "schedule", 2);
            _maxVehiclesBinding = new ValueBinding<int>(kGroup, "maxVehicles", 1);

            AddBinding(_selectedLineBinding);
            AddBinding(_nightVehicleCountBinding);
            AddBinding(_scheduleBinding);
            AddBinding(_maxVehiclesBinding);

            AddBinding(new TriggerBinding<int, int>(kGroup, "setNightVehicles", (lineIndex, count) =>
            {
                string lineKey = GetStableKey(lineIndex);
                if (lineKey == null)
                {
                    Mod.log.Warn($"Could not find stable key for line {lineIndex}");
                    return;
                }
                Mod.log.Info($"Set night vehicles for line {lineKey}: {count}");
                NightVehicleSettings.LineNightVehicles[lineKey] = count;
                _nightVehicleCountBinding.Update(count);
                World.GetOrCreateSystemManaged<NightVehiclesPersistenceSystem>().Save();
            }));

            _transportLineQuery = GetEntityQuery(
                ComponentType.ReadOnly<TransportLine>()
            );
        }

        private string GetStableKey(int entityIndex)
        {
            var entities = _transportLineQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            string result = null;
            foreach (var entity in entities)
            {
                if (entity.Index != entityIndex) continue;
                result = GetStableKeyFromEntity(entity);
                break;
            }
            entities.Dispose();
            return result;
        }

        private string GetStableKeyFromEntity(Entity entity)
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
            if (_selectedInfoSystem == null)
                _selectedInfoSystem = World.GetExistingSystemManaged<SelectedInfoUISystem>();
            if (_selectedInfoSystem == null) return;

            var selectedEntity = _selectedInfoSystem.selectedEntity;
            if (selectedEntity == Entity.Null || !EntityManager.HasComponent<TransportLine>(selectedEntity))
            {
                _selectedLineBinding.Update(-1);
                return;
            }

            _selectedLineBinding.Update(selectedEntity.Index);

            if (EntityManager.HasComponent<Route>(selectedEntity))
            {
                var route = EntityManager.GetComponentData<Route>(selectedEntity);
                int schedule;
                if (RouteUtils.CheckOption(route, RouteOption.Day))
                    schedule = 0;
                else if (RouteUtils.CheckOption(route, RouteOption.Night))
                    schedule = 1;
                else
                    schedule = 2;
                _scheduleBinding.Update(schedule);
            }

            if (EntityManager.HasComponent<RouteInfo>(selectedEntity) &&
                EntityManager.HasComponent<TransportLine>(selectedEntity))
            {
                var routeInfo = EntityManager.GetComponentData<RouteInfo>(selectedEntity);
                var transportLine = EntityManager.GetComponentData<TransportLine>(selectedEntity);
                int maxVehicles = Game.Simulation.TransportLineSystem.CalculateVehicleCount(
                    transportLine.m_VehicleInterval, routeInfo.m_Duration);
                _maxVehiclesBinding.Update(maxVehicles);
            }

            string lineKey = GetStableKeyFromEntity(selectedEntity);
            if (lineKey != null && NightVehicleSettings.LineNightVehicles.TryGetValue(lineKey, out int nightCount))
                _nightVehicleCountBinding.Update(nightCount);
            else
                _nightVehicleCountBinding.Update(1);
        }
    }
}