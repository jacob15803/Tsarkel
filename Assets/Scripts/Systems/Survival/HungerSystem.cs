using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Systems.Survival
{
    /// <summary>
    /// Manages player hunger, depletion, and starvation effects.
    /// </summary>
    public class HungerSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Survival configuration asset")]
        [SerializeField] private SurvivalConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Health system reference for starvation damage")]
        [SerializeField] private HealthSystem healthSystem;
        
        private float currentHunger;
        private float timeSinceLastDepletion;
        
        /// <summary>
        /// Current hunger value.
        /// </summary>
        public float CurrentHunger => currentHunger;
        
        /// <summary>
        /// Maximum hunger value.
        /// </summary>
        public float MaxHunger => config != null ? config.MaxHunger : 100f;
        
        /// <summary>
        /// Hunger as a percentage (0-1).
        /// </summary>
        public float HungerPercentage => MaxHunger > 0 ? currentHunger / MaxHunger : 0f;
        
        /// <summary>
        /// Whether the player is starving (hunger at 0).
        /// </summary>
        public bool IsStarving => currentHunger <= 0f;
        
        private void Start()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the hunger system.
        /// </summary>
        public void Initialize()
        {
            if (config != null)
            {
                currentHunger = config.StartingHunger;
            }
            else
            {
                currentHunger = 100f;
            }
            
            timeSinceLastDepletion = 0f;
            EventManager.Instance.InvokePlayerHungerChanged(currentHunger, MaxHunger);
        }
        
        private void Update()
        {
            if (config == null) return;
            
            timeSinceLastDepletion += Time.deltaTime;
            
            // Deplete hunger over time
            if (currentHunger > 0f)
            {
                DepleteHunger(config.HungerDepletionPerSecond * Time.deltaTime);
            }
            
            // Apply starvation damage if hunger is at 0
            if (IsStarving && healthSystem != null && config.StarvationDamagePerSecond > 0f)
            {
                healthSystem.TakeDamage(config.StarvationDamagePerSecond * Time.deltaTime, "Starvation");
            }
        }
        
        /// <summary>
        /// Depletes hunger by the specified amount.
        /// </summary>
        private void DepleteHunger(float amount)
        {
            currentHunger = Mathf.Max(0f, currentHunger - amount);
            EventManager.Instance.InvokePlayerHungerChanged(currentHunger, MaxHunger);
        }
        
        /// <summary>
        /// Restores hunger by the specified amount (for consumables).
        /// </summary>
        public void RestoreHunger(float amount)
        {
            currentHunger = Mathf.Min(MaxHunger, currentHunger + amount);
            EventManager.Instance.InvokePlayerHungerChanged(currentHunger, MaxHunger);
        }
        
        /// <summary>
        /// Sets hunger to a specific value.
        /// </summary>
        public void SetHunger(float hunger)
        {
            currentHunger = Mathf.Clamp(hunger, 0f, MaxHunger);
            EventManager.Instance.InvokePlayerHungerChanged(currentHunger, MaxHunger);
        }
    }
}
