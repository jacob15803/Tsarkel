using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.Systems.ObjectPooling;

namespace Tsarkel.Environment
{
    /// <summary>
    /// Spawns debris during tsunami events using object pooling.
    /// </summary>
    public class DebrisSpawner : MonoBehaviour
    {
        [Header("Pool Settings")]
        [Tooltip("Object pool for debris")]
        [SerializeField] private ObjectPool debrisPool;
        
        [Header("Spawn Settings")]
        [Tooltip("Number of debris to spawn during tsunami")]
        [SerializeField] private int debrisCount = 20;
        
        [Tooltip("Spawn radius around player")]
        [SerializeField] private float spawnRadius = 50f;
        
        [Tooltip("Minimum spawn distance from player")]
        [SerializeField] private float minSpawnDistance = 10f;
        
        [Tooltip("Spawn height above water")]
        [SerializeField] private float spawnHeight = 5f;
        
        private bool isSpawning = false;
        private Transform playerTransform;
        
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        private void OnEnable()
        {
            EventManager.Instance.OnTsunamiWave += HandleTsunamiWave;
            EventManager.Instance.OnTsunamiWaveEnd += HandleTsunamiWaveEnd;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnTsunamiWave -= HandleTsunamiWave;
            EventManager.Instance.OnTsunamiWaveEnd -= HandleTsunamiWaveEnd;
        }
        
        /// <summary>
        /// Handles tsunami wave event - spawns debris.
        /// </summary>
        private void HandleTsunamiWave(float waveIntensity)
        {
            isSpawning = true;
            SpawnDebris();
        }
        
        /// <summary>
        /// Handles tsunami wave end event - cleans up debris.
        /// </summary>
        private void HandleTsunamiWaveEnd()
        {
            isSpawning = false;
            
            if (debrisPool != null)
            {
                debrisPool.ReturnAll();
            }
        }
        
        /// <summary>
        /// Spawns debris around the player.
        /// </summary>
        private void SpawnDebris()
        {
            if (debrisPool == null || playerTransform == null) return;
            
            for (int i = 0; i < debrisCount; i++)
            {
                Vector3 spawnPosition = GetRandomSpawnPosition();
                GameObject debris = debrisPool.Get(spawnPosition, Quaternion.Euler(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                ));
                
                if (debris != null)
                {
                    // Add random force to debris
                    Rigidbody rb = debris.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(
                            new Vector3(
                                Random.Range(-5f, 5f),
                                Random.Range(5f, 15f),
                                Random.Range(-5f, 5f)
                            ),
                            ForceMode.Impulse
                        );
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets a random spawn position around the player.
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            if (playerTransform == null)
            {
                return Vector3.zero;
            }
            
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, spawnRadius);
            
            Vector3 position = playerTransform.position;
            position.x += randomCircle.x * distance;
            position.z += randomCircle.y * distance;
            position.y += spawnHeight;
            
            return position;
        }
    }
}
