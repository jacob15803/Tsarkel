using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Tribal;

namespace Tsarkel.Systems.Tribal
{
    /// <summary>
    /// Tracks and manages tribal hostility level.
    /// Handles hostility stages and escalation.
    /// </summary>
    public class TribalHostilityMeter : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tribal configuration asset")]
        [SerializeField] private TribalConfig config;
        
        private float currentHostility = 0f;
        private bool isInTerritory = false;
        
        /// <summary>
        /// Current hostility level (0-100).
        /// </summary>
        public float CurrentHostility => currentHostility;
        
        /// <summary>
        /// Hostility as a percentage (0-1).
        /// </summary>
        public float HostilityPercentage => config != null ? currentHostility / config.MaxHostility : 0f;
        
        /// <summary>
        /// Current hostility stage.
        /// </summary>
        public HostilityStage CurrentStage
        {
            get
            {
                if (config == null) return HostilityStage.Neutral;
                
                if (currentHostility >= config.HostileStageThreshold)
                    return HostilityStage.Hostile;
                else if (currentHostility >= config.EscalationStageThreshold)
                    return HostilityStage.Escalation;
                else if (currentHostility >= config.WarningStageThreshold)
                    return HostilityStage.Warning;
                else
                    return HostilityStage.Neutral;
            }
        }
        
        public enum HostilityStage
        {
            Neutral,    // 0-40
            Warning,    // 40-70
            Escalation, // 70-100
            Hostile     // 70-100 (same as escalation, but more aggressive)
        }
        
        private void Update()
        {
            if (config == null) return;
            
            // Update hostility based on territory presence
            if (isInTerritory)
            {
                IncreaseHostility(config.HostilityIncreasePerSecond * Time.deltaTime);
            }
            else
            {
                DecreaseHostility(config.HostilityDecreasePerSecond * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Sets whether player is in tribal territory.
        /// </summary>
        public void SetInTerritory(bool inTerritory)
        {
            isInTerritory = inTerritory;
        }
        
        /// <summary>
        /// Increases hostility by the specified amount.
        /// </summary>
        public void IncreaseHostility(float amount)
        {
            if (config == null) return;
            
            float oldHostility = currentHostility;
            currentHostility = Mathf.Min(config.MaxHostility, currentHostility + amount);
            
            if (Mathf.Abs(currentHostility - oldHostility) > 0.1f)
            {
                EventManager.Instance.InvokeTribalHostilityChanged(currentHostility);
            }
        }
        
        /// <summary>
        /// Decreases hostility by the specified amount.
        /// </summary>
        public void DecreaseHostility(float amount)
        {
            if (config == null) return;
            
            float oldHostility = currentHostility;
            currentHostility = Mathf.Max(0f, currentHostility - amount);
            
            if (Mathf.Abs(currentHostility - oldHostility) > 0.1f)
            {
                EventManager.Instance.InvokeTribalHostilityChanged(currentHostility);
            }
        }
        
        /// <summary>
        /// Sets hostility to a specific value.
        /// </summary>
        public void SetHostility(float hostility)
        {
            if (config == null) return;
            
            currentHostility = Mathf.Clamp(hostility, 0f, config.MaxHostility);
            EventManager.Instance.InvokeTribalHostilityChanged(currentHostility);
        }
    }
}
