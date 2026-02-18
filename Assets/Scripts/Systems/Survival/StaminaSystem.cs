using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Systems.Survival
{
    /// <summary>
    /// Manages player stamina, depletion, and regeneration.
    /// </summary>
    public class StaminaSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Survival configuration asset")]
        [SerializeField] private SurvivalConfig config;
        
        private float currentStamina;
        private float timeSinceLastDepletion;
        private bool isSprinting;
        
        /// <summary>
        /// Current stamina value.
        /// </summary>
        public float CurrentStamina => currentStamina;
        
        /// <summary>
        /// Maximum stamina value.
        /// </summary>
        public float MaxStamina => config != null ? config.MaxStamina : 100f;
        
        /// <summary>
        /// Stamina as a percentage (0-1).
        /// </summary>
        public float StaminaPercentage => MaxStamina > 0 ? currentStamina / MaxStamina : 0f;
        
        /// <summary>
        /// Whether the player has enough stamina to sprint.
        /// </summary>
        public bool CanSprint => currentStamina >= (config != null ? config.MinStaminaToSprint : 10f);
        
        private void Start()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the stamina system with max stamina.
        /// </summary>
        public void Initialize()
        {
            currentStamina = MaxStamina;
            timeSinceLastDepletion = 0f;
            isSprinting = false;
            EventManager.Instance.InvokePlayerStaminaChanged(currentStamina, MaxStamina);
        }
        
        private void Update()
        {
            timeSinceLastDepletion += Time.deltaTime;
            
            // Regenerate stamina if not sprinting and enough time has passed
            if (!isSprinting && config != null && timeSinceLastDepletion >= config.StaminaRegenDelay && currentStamina < MaxStamina)
            {
                RegenerateStamina(config.StaminaRegenPerSecond * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Starts consuming stamina (called when player starts sprinting).
        /// </summary>
        public void StartSprinting()
        {
            if (!CanSprint) return;
            
            isSprinting = true;
            timeSinceLastDepletion = 0f;
        }
        
        /// <summary>
        /// Stops consuming stamina (called when player stops sprinting).
        /// </summary>
        public void StopSprinting()
        {
            isSprinting = false;
            timeSinceLastDepletion = 0f;
        }
        
        /// <summary>
        /// Updates stamina consumption while sprinting (called every frame during sprint).
        /// </summary>
        public void UpdateSprinting()
        {
            if (!isSprinting || config == null) return;
            
            if (currentStamina > 0f)
            {
                DepleteStamina(config.StaminaDepletionPerSecond * Time.deltaTime);
                
                // Auto-stop sprinting if stamina is depleted
                if (currentStamina <= 0f)
                {
                    StopSprinting();
                }
            }
        }
        
        /// <summary>
        /// Depletes stamina by the specified amount.
        /// </summary>
        private void DepleteStamina(float amount)
        {
            currentStamina = Mathf.Max(0f, currentStamina - amount);
            timeSinceLastDepletion = 0f;
            EventManager.Instance.InvokePlayerStaminaChanged(currentStamina, MaxStamina);
        }
        
        /// <summary>
        /// Regenerates stamina over time (internal method).
        /// </summary>
        private void RegenerateStamina(float regenAmount)
        {
            currentStamina = Mathf.Min(MaxStamina, currentStamina + regenAmount);
            EventManager.Instance.InvokePlayerStaminaChanged(currentStamina, MaxStamina);
        }
        
        /// <summary>
        /// Restores stamina by the specified amount (for consumables).
        /// </summary>
        public void RestoreStamina(float amount)
        {
            currentStamina = Mathf.Min(MaxStamina, currentStamina + amount);
            timeSinceLastDepletion = 0f;
            EventManager.Instance.InvokePlayerStaminaChanged(currentStamina, MaxStamina);
        }
        
        /// <summary>
        /// Sets stamina to a specific value.
        /// </summary>
        public void SetStamina(float stamina)
        {
            currentStamina = Mathf.Clamp(stamina, 0f, MaxStamina);
            EventManager.Instance.InvokePlayerStaminaChanged(currentStamina, MaxStamina);
        }
    }
}
