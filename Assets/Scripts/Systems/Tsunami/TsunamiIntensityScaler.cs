using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Systems.Tsunami
{
    /// <summary>
    /// Calculates tsunami intensity based on in-game days.
    /// Integrates with TimeManager to provide day-based scaling.
    /// </summary>
    public class TsunamiIntensityScaler : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tsunami configuration asset")]
        [SerializeField] private TsunamiConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Time manager reference")]
        [SerializeField] private Managers.TimeManager timeManager;
        
        private float daysPassed = 0f;
        private float lastTimeOfDay = 0f;
        private float totalTimePassed = 0f;
        
        /// <summary>
        /// Current intensity multiplier based on days passed.
        /// </summary>
        public float CurrentIntensityMultiplier { get; private set; } = 1f;
        
        /// <summary>
        /// Days passed since game start.
        /// </summary>
        public float DaysPassed => daysPassed;
        
        private void Start()
        {
            if (timeManager == null)
            {
                timeManager = FindObjectOfType<Managers.TimeManager>();
            }
            
            if (config == null)
            {
                Debug.LogWarning("TsunamiIntensityScaler: TsunamiConfig is not assigned!");
            }
            
            if (timeManager != null)
            {
                lastTimeOfDay = timeManager.TimeOfDay;
            }
        }
        
        private void Update()
        {
            if (timeManager == null || config == null) return;
            
            // Calculate days passed based on TimeManager
            UpdateDaysPassed();
            
            // Calculate intensity multiplier
            if (config.UseDayBasedScaling)
            {
                CurrentIntensityMultiplier = config.GetIntensityMultiplierByDays(daysPassed);
            }
        }
        
        /// <summary>
        /// Updates the days passed counter based on TimeManager.
        /// </summary>
        private void UpdateDaysPassed()
        {
            if (timeManager == null) return;
            
            float currentTimeOfDay = timeManager.TimeOfDay;
            
            // Check if we've passed midnight (wrapped around from 1.0 to 0.0)
            if (currentTimeOfDay < lastTimeOfDay)
            {
                daysPassed += 1f;
            }
            
            lastTimeOfDay = currentTimeOfDay;
        }
        
        /// <summary>
        /// Gets the current intensity multiplier.
        /// </summary>
        public float GetIntensityMultiplier()
        {
            return CurrentIntensityMultiplier;
        }
        
        /// <summary>
        /// Sets days passed (for testing or save/load).
        /// </summary>
        public void SetDaysPassed(float days)
        {
            daysPassed = Mathf.Max(0f, days);
            
            if (config != null && config.UseDayBasedScaling)
            {
                CurrentIntensityMultiplier = config.GetIntensityMultiplierByDays(daysPassed);
            }
        }
    }
}
