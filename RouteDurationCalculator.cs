using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Unity.Entities;
using Unity.Mathematics;

namespace NightVehiclesMod
{
    public static class RouteDurationCalculator
    {
        public static float CalculateStableDuration(
            EntityManager entityManager,
            Entity routeEntity,
            TransportLineData transportLineData)
        {
            if (!entityManager.HasBuffer<RouteWaypoint>(routeEntity) ||
                !entityManager.HasBuffer<RouteSegment>(routeEntity))
                return 0f;

            var waypoints = entityManager.GetBuffer<RouteWaypoint>(routeEntity);
            var segments = entityManager.GetBuffer<RouteSegment>(routeEntity);
            if (waypoints.Length == 0)
                return 0f;

            int startIndex = 0;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (entityManager.HasComponent<VehicleTiming>(waypoints[i].m_Waypoint))
                {
                    startIndex = i;
                    break;
                }
            }

            float duration = 0f;
            for (int j = 0; j < waypoints.Length; j++)
            {
                int2 indices = new int2(startIndex + j, startIndex + j + 1);
                indices = math.select(indices, indices - waypoints.Length, indices >= waypoints.Length);

                Entity waypoint = waypoints[indices.y].m_Waypoint;
                Entity segment = segments[indices.x].m_Segment;

                if (entityManager.HasComponent<PathInformation>(segment))
                    duration += entityManager.GetComponentData<PathInformation>(segment).m_Duration;

                if (entityManager.HasComponent<VehicleTiming>(waypoint))
                    duration += transportLineData.m_StopDuration;
            }

            return duration;
        }
    }
}
