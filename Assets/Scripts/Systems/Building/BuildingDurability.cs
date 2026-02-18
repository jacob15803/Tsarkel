using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Buildings;

namespace Tsarkel.Systems.Building
{
    /// <summary>
    /// Manages building health/durability and handles damage from tsunamis.
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
