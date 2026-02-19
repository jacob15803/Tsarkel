using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Buildings;
using Tsarkel.Systems.Tsunami;

namespace Tsarkel.Systems.Building
{
    /// <summary>
    /// Manages building health/durability and handles damage from tsunamis.
    /// Includes elevation-based damage reduction for high-elevation structures.
    /// </summary>
    public class BuildingDurability : MonoBehaviour
    {
        [Header("State")]
        [Tooltip("Current health/durability")]
        [SerializeField] private float currentHealth;
        
        [Tooltip("Maximum health/durability")]
        [SerializeField] private float maxHealth;
        
        [Tooltip("Damage resistance multiplier")]
        [SerializeField] private float damageResistance = 1f;
        
        [Header("Elevation-Based Protection")]
        [Tooltip("Maximum elevation above water for full protection (in units)")]
        [SerializeField] private float maxProtectionElevation = 20f;
        
        [Tooltip("Maximum damage reduction from elevation (0-1, where 1 = 100% reduction)")]
        [SerializeField] private float maxElevationDamageReduction = 0.7f; // 70% reduction at max elevation
        
        [Header("Dependencies")]
        [Tooltip("Safe elevation detector reference (for elevation calculations)")]
        [SerializeField] private SafeElevationDetector elevationDetector;
        
        private BuildingData buildingData;
        private bool isInitialized = false;
        
        /// <summary>
        /// Current health value.
        /// </summary>
        public float CurrentHealth => currentHealth;
        
        /// <summary>
        /// Maximum health value.
        /// </summary>
        public float MaxHealth => maxHealth;
        
        /// <summary>
        /// Health as a percentage (0-1).
        /// </summary>
        public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        
        /// <summary>
        /// Whether the building is destroyed.
        /// </summary>
        public bool IsDestroyed => currentHealth <= 0f;
        
        /// <summary>
        /// Initializes the building durability with building data.
        /// </summary>
        public void Initialize(BuildingData data)
        {
            if (data == null)
            {
                Debug.LogWarning("BuildingDurability: Cannot initialize with null building data.");
                return;
            }
            
            buildingData = data;
            maxHealth = data.MaxDurability;
            currentHealth = maxHealth;
            damageResistance = data.DamageResistance;
            isInitialized = true;
        }
        
        /// <summary>
        /// Applies damage to the building.
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        public void TakeDamage(float damage)
        {
            if (!isInitialized || IsDestroyed) return;
            
            // Apply damage resistance
            float actualDamage = damage / damageResistance;
            
            // Apply elevation-based damage reduction
            actualDamage = ApplyElevationDamageReduction(actualDamage);
            
            currentHealth = Mathf.Max(0f, currentHealth - actualDamage);
            
            // Invoke damage event
            EventManager.Instance.InvokeBuildingDamaged(gameObject, actualDamage, currentHealth, maxHealth);
            
            // Check for destruction
            if (currentHealth <= 0f)
            {
                DestroyBuilding();
            }
        }
        
        /// <summary>
        /// Applies elevation-based damage reduction.
        /// Higher structures take less damage from tsunamis.
        /// </summary>
        /// <param name="damage">Base damage amount</param>
        /// <returns>Damage after elevation reduction</returns>
        private float ApplyElevationDamageReduction(float damage)
        {
            if (elevationDetector == null)
            {
                elevationDetector = FindObjectOfType<SafeElevationDetector>();
            }
            
            if (elevationDetector == null) return damage;
            
            // Get building elevation
            float buildingElevation = transform.position.y;
            float waterLevel = elevationDetector.CurrentWaterLevel;
            float elevationAboveWater = buildingElevation - waterLevel;
            
            // Calculate protection factor (0-1)
            float protectionFactor = Mathf.Clamp01(elevationAboveWater / maxProtectionElevation);
            
            // Apply damage reduction
            float damageReduction = protectionFactor * maxElevationDamageReduction;
            float reducedDamage = damage * (1f - damageReduction);
            
            return reducedDamage;
        }
        
        /// <summary>
        /// Repairs the building by the specified amount.
        /// </summary>
        /// <param name="repairAmount">Amount to repair</param>
        public void Repair(float repairAmount)
        {
            if (!isInitialized || IsDestroyed) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + repairAmount);
            
            EventManager.Instance.InvokeBuildingDamaged(gameObject, -repairAmount, currentHealth, maxHealth);
        }
        
        /// <summary>
        /// Destroys the building.
        /// </summary>
        private void DestroyBuilding()
        {
            EventManager.Instance.InvokeBuildingDestroyed(gameObject);
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Sets health to a specific value (for initialization or cheats).
        /// </summary>
        public void SetHealth(float health)
        {
            if (!isInitialized) return;
            
            currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            
            if (currentHealth <= 0f && !IsDestroyed)
            {
                DestroyBuilding();
            }
        }
    }
}
