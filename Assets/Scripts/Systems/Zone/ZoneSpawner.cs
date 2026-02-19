using UnityEngine;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Systems.Zone
{
    /// <summary>
    /// Manages spawn points within a zone.
    /// Provides distance-based spawning with randomization.
    /// </summary>
    public class ZoneSpawner : MonoBehaviour
    {
        [Header("Zone Configuration")]
        [Tooltip("Zone data asset")]
        [SerializeField] private ZoneData zoneData;
        
        [Header("Spawn Settings")]
        [Tooltip("Spawn points in this zone")]
        [SerializeField] private Transform[] spawnPoints;
        
        [Tooltip("Randomization radius around spawn points")]
        [SerializeField] private float spawnRadius = 5f;
        
        [Tooltip("Minimum distance between spawns")]
        [SerializeField] private float minSpawnDistance = 10f;
        
        /// <summary>
        /// Gets a random spawn position within this zone.
        /// </summary>
        public Vector3 GetRandomSpawnPosition()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                // Fallback to transform position
                return transform.position + Random.insideUnitSphere * spawnRadius;
            }
            
            // Pick random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Add randomization within radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = spawnPoint.position;
            spawnPosition.x += randomCircle.x;
            spawnPosition.z += randomCircle.y;
            
            return spawnPosition;
        }
        
        /// <summary>
        /// Gets a spawn position near a specific point (for area-based spawning).
        /// </summary>
        public Vector3 GetSpawnPositionNear(Vector3 center, float maxDistance)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(0f, maxDistance);
            
            Vector3 spawnPosition = center;
            spawnPosition.x += randomCircle.x * distance;
            spawnPosition.z += randomCircle.y * distance;
            
            return spawnPosition;
        }
        
        /// <summary>
        /// Gets all spawn points in this zone.
        /// </summary>
        public Transform[] GetSpawnPoints()
        {
            return spawnPoints;
        }
        
        /// <summary>
        /// Gets the zone data for this spawner.
        /// </summary>
        public ZoneData GetZoneData()
        {
            return zoneData;
        }
    }
}
