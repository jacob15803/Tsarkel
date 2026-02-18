using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Systems.Survival
{
    /// <summary>
    /// Manages player hydration, depletion, and dehydration effects.
    /// </summary>
    public class HydrationSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Survival configuration asset")]
        [SerializeField] private SurvivalConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Health system reference for dehydration damage")]
        [SerializeField] private HealthSystem healthSystem;
        
        private float currentHydration;
        private float timeSinceLastDepletion;
        
        /// <summary>
        /// Current hydration value.
        /// </summary>
        public float CurrentHydration => currentHydration;
        
        /// <summary>
        /// Maximum hydration value.
        /// </summary>
        public float MaxHydration => config != null ? config.MaxHydration : 100f;
        
        /// <summary>
        /// Hydration as a percentage (0-1).
        /// </summary>
        public float HydrationPercentage => MaxHydration > 0 ? currentHydration / MaxHydration : 0f;
        
        /// <summary>
        /// Whether the player is dehydrated (hydration at 0).
        /// </summary>
        public bool IsDehydrated => currentHydration <= 0f;
        
        private void Start()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the hydration system.
        /// </summary>
        public void Initialize()
        {
            if (config != null)
            {
                currentHydration = config.StartingHydration;
            }
            else
            {
                currentHydration = 100f;
            }
            
            timeSinceLastDepletion = 0f;
            EventManager.Instance.InvokePlayerHydrationChanged(currentHydration, MaxHydration);
        }
        
        private void Update()
        {
            if (config == null) return;
            
            timeSinceLastDepletion += Time.deltaTime;
            
            // Deplete hydration over time
            if (currentHydration > 0f)
            {
                DepleteHydration(config.HydrationDepletionPerSecond * Time.deltaTime);
            }
            
            // Apply dehydration damage if hydration is at 0
            if (IsDehydrated && healthSystem != null && config.DehydrationDamagePerSecond > 0f)
            {
                healthSystem.TakeDamage(config.DehydrationDamagePerSecond * Time.deltaTime, "Dehydration");
            }
        }
        
        /// <summary>
        /// Depletes hydration by the specified amount.
        /// </summary>
        private void DepleteHydration(float amount)
        {
            currentHydration = Mathf.Max(0f, currentHydration - amount);
            EventManager.Instance.InvokePlayerHydrationChanged(currentHydration, MaxHydration);
        }
        
        /// <summary>
        /// Restores hydration by the specified amount (for consumables).
        /// </summary>
        public void RestoreHydration(float amount)
        {
            currentHydration = Mathf.Min(MaxHydration, currentHydration + amount);
            EventManager.Instance.InvokePlayerHydrationChanged(currentHydration, MaxHydration);
        }
        
        /// <summary>
        /// Sets hydration to a specific value.
        /// </summary>
        public void SetHydration(float hydration)
        {
            currentHydration = Mathf.Clamp(hydration, 0f, MaxHydration);
            EventManager.Instance.InvokePlayerHydrationChanged(currentHydration, MaxHydration);
        }
    }
}
