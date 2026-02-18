using UnityEngine;

namespace Tsarkel.ScriptableObjects.Config
{
    /// <summary>
    /// Configuration data for survival systems (health, stamina, hunger, hydration).
    /// </summary>
    [CreateAssetMenu(fileName = "SurvivalConfig", menuName = "Tsarkel/Config/Survival Config")]
    public class SurvivalConfig : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("Maximum health value")]
        [SerializeField] private float maxHealth = 100f;
        
        [Tooltip("Health regeneration per second (when not taking damage)")]
        [SerializeField] private float healthRegenPerSecond = 0.5f;
        
        [Tooltip("Delay before health regeneration starts after taking damage (seconds)")]
        [SerializeField] private float healthRegenDelay = 5f;
        
        [Header("Stamina")]
        [Tooltip("Maximum stamina value")]
        [SerializeField] private float maxStamina = 100f;
        
        [Tooltip("Stamina depletion per second while sprinting")]
        [SerializeField] private float staminaDepletionPerSecond = 20f;
        
        [Tooltip("Stamina regeneration per second (when not sprinting)")]
        [SerializeField] private float staminaRegenPerSecond = 15f;
        
        [Tooltip("Delay before stamina regeneration starts after depletion (seconds)")]
        [SerializeField] private float staminaRegenDelay = 1f;
        
        [Tooltip("Minimum stamina required to sprint")]
        [SerializeField] private float minStaminaToSprint = 10f;
        
        [Header("Hunger")]
        [Tooltip("Maximum hunger value (100 = fully fed)")]
        [SerializeField] private float maxHunger = 100f;
        
        [Tooltip("Starting hunger value")]
        [SerializeField] private float startingHunger = 100f;
        
        [Tooltip("Hunger depletion per second")]
        [SerializeField] private float hungerDepletionPerSecond = 0.5f;
        
        [Tooltip("Health damage per second when hunger is at 0")]
        [SerializeField] private float starvationDamagePerSecond = 2f;
        
        [Header("Hydration")]
        [Tooltip("Maximum hydration value (100 = fully hydrated)")]
        [SerializeField] private float maxHydration = 100f;
        
        [Tooltip("Starting hydration value")]
        [SerializeField] private float startingHydration = 100f;
        
        [Tooltip("Hydration depletion per second")]
        [SerializeField] private float hydrationDepletionPerSecond = 0.8f;
        
        [Tooltip("Health damage per second when hydration is at 0")]
        [SerializeField] private float dehydrationDamagePerSecond = 3f;
        
        [Header("Damage")]
        [Tooltip("Base damage multiplier for all damage sources")]
        [SerializeField] private float damageMultiplier = 1f;

        // Public properties
        public float MaxHealth => maxHealth;
        public float HealthRegenPerSecond => healthRegenPerSecond;
        public float HealthRegenDelay => healthRegenDelay;
        public float MaxStamina => maxStamina;
        public float StaminaDepletionPerSecond => staminaDepletionPerSecond;
        public float StaminaRegenPerSecond => staminaRegenPerSecond;
        public float StaminaRegenDelay => staminaRegenDelay;
        public float MinStaminaToSprint => minStaminaToSprint;
        public float MaxHunger => maxHunger;
        public float StartingHunger => startingHunger;
        public float HungerDepletionPerSecond => hungerDepletionPerSecond;
        public float StarvationDamagePerSecond => starvationDamagePerSecond;
        public float MaxHydration => maxHydration;
        public float StartingHydration => startingHydration;
        public float HydrationDepletionPerSecond => hydrationDepletionPerSecond;
        public float DehydrationDamagePerSecond => dehydrationDamagePerSecond;
        public float DamageMultiplier => damageMultiplier;
    }
}
