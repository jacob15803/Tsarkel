using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;
using Tsarkel.Player;

namespace Tsarkel.Systems.Tsunami
{
    /// <summary>
    /// Central manager for tsunami events.
    /// Handles timing, phases, and coordination of tsunami systems.
    /// </summary>
    public class TsunamiManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tsunami configuration asset")]
        [SerializeField] private TsunamiConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Water controller reference")]
        [SerializeField] private WaterController waterController;
        
        [Tooltip("Safe elevation detector reference")]
        [SerializeField] private SafeElevationDetector elevationDetector;
        
        [Tooltip("Player controller reference")]
        [SerializeField] private PlayerController playerController;
        
        [Tooltip("Tsunami intensity scaler reference (for day-based scaling)")]
        [SerializeField] private TsunamiIntensityScaler intensityScaler;
        
        [Header("State")]
        [Tooltip("Current tsunami event count (for intensity scaling)")]
        [SerializeField] private int eventCount = 0;
        
        public enum TsunamiPhase
        {
            Idle,
            Warning,
            Wave,
            Receding
        }
        
        private TsunamiPhase currentPhase = TsunamiPhase.Idle;
        private float timeUntilNextTsunami;
        private float phaseTimer;
        private float currentIntensityMultiplier = 1f;
        
        /// <summary>
        /// Current tsunami phase.
        /// </summary>
        public TsunamiPhase CurrentPhase => currentPhase;
        
        /// <summary>
        /// Time until next tsunami (in seconds).
        /// </summary>
        public float TimeUntilNextTsunami => timeUntilNextTsunami;
        
        /// <summary>
        /// Current intensity multiplier.
        /// </summary>
        public float CurrentIntensityMultiplier => currentIntensityMultiplier;
        
        private void Start()
        {
            if (config == null)
            {
                Debug.LogError("TsunamiManager: TsunamiConfig is not assigned!");
                return;
            }
            
            if (waterController == null)
            {
                waterController = FindObjectOfType<WaterController>();
            }
            
            if (elevationDetector == null)
            {
                elevationDetector = FindObjectOfType<SafeElevationDetector>();
            }
            
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }
            
            if (intensityScaler == null)
            {
                intensityScaler = FindObjectOfType<TsunamiIntensityScaler>();
            }
            
            // Initialize first tsunami timer
            ResetTsunamiTimer();
        }
        
        private void Update()
        {
            if (config == null) return;
            
            switch (currentPhase)
            {
                case TsunamiPhase.Idle:
                    UpdateIdlePhase();
                    break;
                    
                case TsunamiPhase.Warning:
                    UpdateWarningPhase();
                    break;
                    
                case TsunamiPhase.Wave:
                    UpdateWavePhase();
                    break;
                    
                case TsunamiPhase.Receding:
                    UpdateRecedingPhase();
                    break;
            }
        }
        
        /// <summary>
        /// Updates the idle phase (waiting for next tsunami).
        /// </summary>
        private void UpdateIdlePhase()
        {
            timeUntilNextTsunami -= Time.deltaTime;
            
            if (timeUntilNextTsunami <= 0f)
            {
                StartWarningPhase();
            }
        }
        
        /// <summary>
        /// Starts the warning phase.
        /// </summary>
        private void StartWarningPhase()
        {
            currentPhase = TsunamiPhase.Warning;
            phaseTimer = config.WarningDuration;
            
            // Calculate intensity multiplier (day-based or event-based)
            if (config != null && config.UseDayBasedScaling && intensityScaler != null)
            {
                currentIntensityMultiplier = intensityScaler.GetIntensityMultiplier();
            }
            else
            {
                currentIntensityMultiplier = config.GetIntensityMultiplier(eventCount);
            }
            
            // Lower water level
            if (waterController != null)
            {
                float targetWaterLevel = waterController.CurrentWaterLevel - config.WarningWaterLowerAmount;
                waterController.SetTargetWaterLevel(targetWaterLevel, config.WarningWaterLowerSpeed);
            }
            
            // Invoke warning event
            EventManager.Instance.InvokeTsunamiWarning(config.WarningDuration);
        }
        
        /// <summary>
        /// Updates the warning phase.
        /// </summary>
        private void UpdateWarningPhase()
        {
            phaseTimer -= Time.deltaTime;
            
            if (phaseTimer <= 0f)
            {
                StartWavePhase();
            }
        }
        
        /// <summary>
        /// Starts the wave phase.
        /// </summary>
        private void StartWavePhase()
        {
            currentPhase = TsunamiPhase.Wave;
            
            // Calculate wave intensity
            float waveIntensity = config.BaseWaveIntensity * currentIntensityMultiplier;
            
            // Raise water level
            if (waterController != null)
            {
                float targetWaterLevel = waterController.CurrentWaterLevel + config.WaveWaterRiseAmount * currentIntensityMultiplier;
                waterController.SetTargetWaterLevel(targetWaterLevel, config.WaveWaterRiseSpeed * currentIntensityMultiplier);
            }
            
            // Set phase timer for peak duration
            phaseTimer = config.WavePeakDuration;
            
            // Invoke wave event
            EventManager.Instance.InvokeTsunamiWave(waveIntensity);
            
            // Apply immediate effects
            ApplyWaveEffects();
        }
        
        /// <summary>
        /// Updates the wave phase.
        /// </summary>
        private void UpdateWavePhase()
        {
            phaseTimer -= Time.deltaTime;
            
            // Apply continuous wave effects
            ApplyWaveEffects();
            
            if (phaseTimer <= 0f)
            {
                StartRecedingPhase();
            }
        }
        
        /// <summary>
        /// Starts the receding phase.
        /// </summary>
        private void StartRecedingPhase()
        {
            currentPhase = TsunamiPhase.Receding;
            
            // Lower water back to normal
            if (waterController != null)
            {
                waterController.SetTargetWaterLevel(0f, config.WarningWaterLowerSpeed);
            }
            
            // Wait for water to recede before ending
            phaseTimer = config.WarningWaterLowerAmount / config.WarningWaterLowerSpeed;
        }
        
        /// <summary>
        /// Updates the receding phase.
        /// </summary>
        private void UpdateRecedingPhase()
        {
            phaseTimer -= Time.deltaTime;
            
            if (phaseTimer <= 0f && waterController != null && waterController.CurrentWaterLevel <= 0.1f)
            {
                EndTsunami();
            }
        }
        
        /// <summary>
        /// Applies wave effects (damage, forces, structure destruction).
        /// </summary>
        private void ApplyWaveEffects()
        {
            if (playerController == null || elevationDetector == null || config == null) return;
            
            // Check if player is below safe elevation
            if (!playerController.IsAtSafeElevation())
            {
                // Apply force to player
                Vector3 forceDirection = Vector3.up + (Vector3.forward * 0.3f); // Upward and forward
                float forceMagnitude = config.PlayerForceMultiplier * currentIntensityMultiplier;
                playerController.ApplyForce(forceDirection * forceMagnitude);
                
                // Apply damage to player
                if (playerController.GetComponent<PlayerStats>() != null)
                {
                    float damage = config.PlayerDamagePerSecond * currentIntensityMultiplier * Time.deltaTime;
                    playerController.GetComponent<PlayerStats>().TakeDamage(damage, "Tsunami");
                }
            }
            
            // Destroy low structures
            DestroyLowStructures();
        }
        
        /// <summary>
        /// Destroys structures tagged "LowStructure" that are below safe elevation.
        /// </summary>
        private void DestroyLowStructures()
        {
            GameObject[] lowStructures = GameObject.FindGameObjectsWithTag("LowStructure");
            float safeElevation = elevationDetector != null ? 
                elevationDetector.CurrentWaterLevel + elevationDetector.SafeElevationThreshold : 10f;
            
            foreach (GameObject structure in lowStructures)
            {
                if (structure.transform.position.y < safeElevation)
                {
                    // Check if structure has BuildingDurability component
                    var durability = structure.GetComponent<Building.BuildingDurability>();
                    if (durability != null)
                    {
                        // Apply massive damage to destroy it
                        float damage = config.LowStructureDamageMultiplier * currentIntensityMultiplier * 1000f;
                        durability.TakeDamage(damage);
                    }
                    else
                    {
                        // Destroy immediately if no durability system
                        EventManager.Instance.InvokeBuildingDestroyed(structure);
                        Destroy(structure);
                    }
                }
            }
        }
        
        /// <summary>
        /// Ends the current tsunami and resets for the next one.
        /// </summary>
        private void EndTsunami()
        {
            currentPhase = TsunamiPhase.Idle;
            eventCount++;
            
            EventManager.Instance.InvokeTsunamiWaveEnd();
            ResetTsunamiTimer();
        }
        
        /// <summary>
        /// Resets the timer for the next tsunami.
        /// </summary>
        private void ResetTsunamiTimer()
        {
            if (config != null)
            {
                timeUntilNextTsunami = config.GetRandomInterval();
            }
        }
        
        /// <summary>
        /// Forces a tsunami to start immediately (for testing).
        /// </summary>
        [ContextMenu("Force Tsunami")]
        public void ForceTsunami()
        {
            if (currentPhase == TsunamiPhase.Idle)
            {
                StartWarningPhase();
            }
        }
    }
}
