using UnityEngine;
using Tsarkel.ScriptableObjects.AI;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Spawn point component for predators.
    /// Defines a location where predators can spawn.
    /// </summary>
    public class PredatorSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Predator data asset for this spawn point")]
        [SerializeField] private PredatorData predatorData;
        
        [Tooltip("Spawn radius (randomization area)")]
        [SerializeField] private float spawnRadius = 3f;
        
        [Tooltip("Whether this spawn point is active")]
        [SerializeField] private bool isActive = true;
        
        /// <summary>
        /// Gets the spawn position (with randomization).
        /// </summary>
        public Vector3 GetSpawnPosition()
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position;
            spawnPos.x += randomCircle.x;
            spawnPos.z += randomCircle.y;
            
            return spawnPos;
        }
        
        /// <summary>
        /// Gets the predator data for this spawn point.
        /// </summary>
        public PredatorData GetPredatorData()
        {
            return predatorData;
        }
        
        /// <summary>
        /// Gets whether this spawn point is active.
        /// </summary>
        public bool IsActive => isActive;
        
        /// <summary>
        /// Sets whether this spawn point is active.
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
        }
    }
}
