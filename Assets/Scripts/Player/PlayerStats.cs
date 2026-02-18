using UnityEngine;
using Tsarkel.Systems.Survival;

namespace Tsarkel.Player
{
    /// <summary>
    /// Centralized player statistics manager.
    /// Integrates all survival systems and exposes unified player state.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Survival Systems")]
        [Tooltip("Health system reference")]
        [SerializeField] private HealthSystem healthSystem;
        
        [Tooltip("Stamina system reference")]
        [SerializeField] private StaminaSystem staminaSystem;
        
        [Tooltip("Hunger system reference")]
        [SerializeField] private HungerSystem hungerSystem;
        
        [Tooltip("Hydration system reference")]
        [SerializeField] private HydrationSystem hydrationSystem;
        
        /// <summary>
        /// Health system reference.
        /// </summary>
        public HealthSystem Health => healthSystem;
        
        /// <summary>
        /// Stamina system reference.
        /// </summary>
        public StaminaSystem Stamina => staminaSystem;
        
        /// <summary>
        /// Hunger system reference.
        /// </summary>
        public HungerSystem Hunger => hungerSystem;
        
        /// <summary>
        /// Hydration system reference.
        /// </summary>
        public HydrationSystem Hydration => hydrationSystem;
        
        /// <summary>
        /// Whether the player is alive.
        /// </summary>
        public bool IsAlive => healthSystem != null && !healthSystem.IsDead;
        
        /// <summary>
        /// Whether the player is in critical condition (low health, hunger, or hydration).
        /// </summary>
        public bool IsInCriticalCondition
        {
            get
            {
                if (!IsAlive) return true;
                
                bool lowHealth = healthSystem != null && healthSystem.HealthPercentage < 0.3f;
                bool starving = hungerSystem != null && hungerSystem.IsStarving;
                bool dehydrated = hydrationSystem != null && hydrationSystem.IsDehydrated;
                
                return lowHealth || starving || dehydrated;
            }
        }
        
        private void Awake()
        {
            // Auto-find systems if not assigned
            if (healthSystem == null)
                healthSystem = GetComponent<HealthSystem>();
            
            if (staminaSystem == null)
                staminaSystem = GetComponent<StaminaSystem>();
            
            if (hungerSystem == null)
                hungerSystem = GetComponent<HungerSystem>();
            
            if (hydrationSystem == null)
                hydrationSystem = GetComponent<HydrationSystem>();
        }
        
        /// <summary>
        /// Initializes all survival systems.
        /// </summary>
        public void Initialize()
        {
            if (healthSystem != null)
                healthSystem.Initialize();
            
            if (staminaSystem != null)
                staminaSystem.Initialize();
            
            if (hungerSystem != null)
                hungerSystem.Initialize();
            
            if (hydrationSystem != null)
                hydrationSystem.Initialize();
        }
        
        /// <summary>
        /// Applies damage to the player.
        /// </summary>
        public void TakeDamage(float damage, string damageSource = "Unknown")
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage, damageSource);
            }
        }
        
        /// <summary>
        /// Heals the player.
        /// </summary>
        public void Heal(float amount)
        {
            if (healthSystem != null)
            {
                healthSystem.Heal(amount);
            }
        }
        
        /// <summary>
        /// Restores hunger.
        /// </summary>
        public void RestoreHunger(float amount)
        {
            if (hungerSystem != null)
            {
                hungerSystem.RestoreHunger(amount);
            }
        }
        
        /// <summary>
        /// Restores hydration.
        /// </summary>
        public void RestoreHydration(float amount)
        {
            if (hydrationSystem != null)
            {
                hydrationSystem.RestoreHydration(amount);
            }
        }
        
        /// <summary>
        /// Restores stamina.
        /// </summary>
        public void RestoreStamina(float amount)
        {
            if (staminaSystem != null)
            {
                staminaSystem.RestoreStamina(amount);
            }
        }
    }
}
