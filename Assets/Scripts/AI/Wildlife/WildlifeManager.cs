using System.Collections.Generic;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Systems.Zone;
using Tsarkel.Systems.ObjectPooling;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Central manager for wildlife spawning and management.
    /// Handles zone-based spawning with object pooling.
    /// </summary>
    public class WildlifeManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Predator prefab")]
        [SerializeField] private GameObject predatorPrefab;
        
        [Tooltip("Object pool for predators")]
        [SerializeField] private ObjectPool predatorPool;
        
        [Tooltip("Maximum active predators per zone")]
        [SerializeField] private int maxPredatorsPerZone = 3;
        
        [Tooltip("Spawn check interval (seconds)")]
        [SerializeField] private float spawnCheckInterval = 10f;
        
        [Tooltip("Base spawn rate (predators per minute)")]
        [SerializeField] private float baseSpawnRate = 0.5f;
        
        [Header("Dependencies")]
        [Tooltip("Zone manager reference")]
        [SerializeField] private ZoneManager zoneManager;
        
        private List<GameObject> activePredators = new List<GameObject>();
        private float lastSpawnCheck = 0f;
        private PredatorSpawnPoint[] allSpawnPoints;
        
        private void Start()
        {
            if (zoneManager == null)
            {
                zoneManager = ZoneManager.Instance;
            }
            
            // Find all spawn points
            allSpawnPoints = FindObjectsOfType<PredatorSpawnPoint>();
            
            // Create object pool if not assigned
            if (predatorPool == null && predatorPrefab != null)
            {
                GameObject poolObj = new GameObject("PredatorPool");
                poolObj.transform.SetParent(transform);
                predatorPool = poolObj.AddComponent<ObjectPool>();
                // Note: ObjectPool needs prefab assigned in inspector
            }
        }
        
        private void Update()
        {
            if (Time.time - lastSpawnCheck >= spawnCheckInterval)
            {
                lastSpawnCheck = Time.time;
                CheckSpawnPredators();
            }
        }
        
        /// <summary>
        /// Checks if predators should be spawned.
        /// </summary>
        private void CheckSpawnPredators()
        {
            if (zoneManager == null || allSpawnPoints == null) return;
            
            // Get spawn multiplier from current zone
            float spawnMultiplier = zoneManager.GetCombinedPredatorSpawnMultiplier();
            
            // Calculate spawn chance
            float spawnChance = baseSpawnRate * spawnMultiplier * (spawnCheckInterval / 60f);
            
            // Count active predators in current zones
            int activeCount = GetActivePredatorCount();
            
            // Spawn if under limit and chance succeeds
            if (activeCount < maxPredatorsPerZone && Random.value < spawnChance)
            {
                SpawnPredator();
            }
        }
        
        /// <summary>
        /// Spawns a predator at a random spawn point in current zones.
        /// </summary>
        private void SpawnPredator()
        {
            if (predatorPrefab == null) return;
            
            // Get spawn points in current zones
            List<PredatorSpawnPoint> validSpawnPoints = GetValidSpawnPoints();
            if (validSpawnPoints.Count == 0) return;
            
            // Pick random spawn point
            PredatorSpawnPoint spawnPoint = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
            if (!spawnPoint.IsActive) return;
            
            // Get spawn position
            Vector3 spawnPosition = spawnPoint.GetSpawnPosition();
            
            // Spawn predator
            GameObject predator;
            if (predatorPool != null)
            {
                predator = predatorPool.Get(spawnPosition, Quaternion.identity);
            }
            else
            {
                predator = Instantiate(predatorPrefab, spawnPosition, Quaternion.identity);
            }
            
            if (predator != null)
            {
                // Configure predator
                var predatorAI = predator.GetComponent<PredatorAI>();
                if (predatorAI != null)
                {
                    predatorAI.SetSpawnPoint(spawnPoint.transform);
                }
                
                activePredators.Add(predator);
                EventManager.Instance.InvokePredatorSpawned(predator);
            }
        }
        
        /// <summary>
        /// Gets valid spawn points in current zones.
        /// </summary>
        private List<PredatorSpawnPoint> GetValidSpawnPoints()
        {
            List<PredatorSpawnPoint> validPoints = new List<PredatorSpawnPoint>();
            
            if (allSpawnPoints == null) return validPoints;
            
            foreach (var spawnPoint in allSpawnPoints)
            {
                if (spawnPoint != null && spawnPoint.IsActive)
                {
                    validPoints.Add(spawnPoint);
                }
            }
            
            return validPoints;
        }
        
        /// <summary>
        /// Gets the count of active predators.
        /// </summary>
        private int GetActivePredatorCount()
        {
            // Clean up destroyed predators
            activePredators.RemoveAll(p => p == null);
            return activePredators.Count;
        }
        
        /// <summary>
        /// Returns a predator to the pool.
        /// </summary>
        public void ReturnPredator(GameObject predator)
        {
            if (predator == null) return;
            
            activePredators.Remove(predator);
            
            if (predatorPool != null)
            {
                predatorPool.Return(predator);
            }
            else
            {
                Destroy(predator);
            }
        }
    }
}
