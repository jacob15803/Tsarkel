using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Systems.Survival
{
    /// <summary>
    /// Manages player health, damage, and regeneration.
    /// </summary>
    public class HealthSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Survival configuration asset")]
        [SerializeField] private SurvivalConfig config;
        
        private float currentHealth;
        private float timeSinceLastDamage;
        private bool isDead;
        
        /// <summary>
        /// Current health value.
        /// </summary>
        public float CurrentHealth => currentHealth;
        
        /// <summary>
        /// Maximum health value.
        /// </summary>
        public float MaxHealth => config != null ? config.MaxHealth : 100f;
        
        /// <summary>
        /// Health as a percentage (0-1).
        /// </summary>
        public float HealthPercentage => MaxHealth > 0 ? currentHealth / MaxHealth : 0f;
        
        /// <summary>
        /// Whether the player is dead.
        /// </summary>
        public bool IsDead => isDead;
        
        private void Start()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the health system with max health.
        /// </summary>
        public void Initialize()
        {
            currentHealth = MaxHealth;
            timeSinceLastDamage = 0f;
            isDead = false;
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
        }
        
        private void Update()
        {
            if (isDead) return;
            
            timeSinceLastDamage += Time.deltaTime;
            
            // Regenerate health if enough time has passed since last damage
            if (config != null && timeSinceLastDamage >= config.HealthRegenDelay && currentHealth < MaxHealth)
            {
                RegenerateHealth(config.HealthRegenPerSecond * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Applies damage to the player.
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        /// <param name="damageSource">Source of the damage (for logging/events)</param>
        public void TakeDamage(float damage, string damageSource = "Unknown")
        {
            if (isDead) return;
            
            float actualDamage = damage;
            if (config != null)
            {
                actualDamage *= config.DamageMultiplier;
            }
            
            currentHealth = Mathf.Max(0f, currentHealth - actualDamage);
            timeSinceLastDamage = 0f;
            
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
            EventManager.Instance.InvokePlayerDamaged(actualDamage, damageSource);
            
            if (currentHealth <= 0f && !isDead)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Heals the player by the specified amount.
        /// </summary>
        /// <param name="healAmount">Amount to heal</param>
        public void Heal(float healAmount)
        {
            if (isDead) return;
            
            currentHealth = Mathf.Min(MaxHealth, currentHealth + healAmount);
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
        }
        
        /// <summary>
        /// Regenerates health over time (internal method).
        /// </summary>
        private void RegenerateHealth(float regenAmount)
        {
            currentHealth = Mathf.Min(MaxHealth, currentHealth + regenAmount);
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
        }
        
        /// <summary>
        /// Sets health to a specific value (for initialization or cheats).
        /// </summary>
        public void SetHealth(float health)
        {
            currentHealth = Mathf.Clamp(health, 0f, MaxHealth);
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
            
            if (currentHealth <= 0f && !isDead)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Handles player death.
        /// </summary>
        private void Die()
        {
            isDead = true;
            currentHealth = 0f;
            EventManager.Instance.InvokePlayerDeath();
        }
        
        /// <summary>
        /// Revives the player (for respawn functionality).
        /// </summary>
        public void Revive(float healthPercentage = 1f)
        {
            isDead = false;
            currentHealth = MaxHealth * Mathf.Clamp01(healthPercentage);
            timeSinceLastDamage = 0f;
            EventManager.Instance.InvokePlayerHealthChanged(currentHealth, MaxHealth);
        }
    }
}
