using UnityEngine;

namespace Tsarkel.ScriptableObjects.Config
{
    /// <summary>
    /// Configuration data for tsunami events.
    /// Controls timing, intensity, and behavior of tsunami waves.
    /// </summary>
    [CreateAssetMenu(fileName = "TsunamiConfig", menuName = "Tsarkel/Config/Tsunami Config")]
    public class TsunamiConfig : ScriptableObject
    {
        [Header("Timing")]
        [Tooltip("Minimum time between tsunamis (in seconds)")]
        [SerializeField] private float minInterval = 300f; // 5 minutes
        
        [Tooltip("Maximum time between tsunamis (in seconds)")]
        [SerializeField] private float maxInterval = 600f; // 10 minutes
        
        [Header("Warning Phase")]
        [Tooltip("Duration of warning phase before wave hits (in seconds)")]
        [SerializeField] private float warningDuration = 30f;
        
        [Tooltip("How much the water level lowers during warning phase (in units)")]
        [SerializeField] private float warningWaterLowerAmount = 5f;
        
        [Tooltip("Speed at which water lowers during warning (units per second)")]
        [SerializeField] private float warningWaterLowerSpeed = 0.5f;
        
        [Header("Wave Phase")]
        [Tooltip("Base intensity of the tsunami wave (affects force and damage)")]
        [SerializeField] private float baseWaveIntensity = 1f;
        
        [Tooltip("How high the water rises during wave phase (in units)")]
        [SerializeField] private float waveWaterRiseAmount = 10f;
        
        [Tooltip("Speed at which water rises during wave (units per second)")]
        [SerializeField] private float waveWaterRiseSpeed = 2f;
        
        [Tooltip("Duration the wave stays at peak height (in seconds)")]
        [SerializeField] private float wavePeakDuration = 10f;
        
        [Header("Intensity Scaling")]
        [Tooltip("Whether tsunami intensity increases over time")]
        [SerializeField] private bool enableIntensityScaling = true;
        
        [Tooltip("Intensity multiplier increase per tsunami event")]
        [SerializeField] private float intensityIncreasePerEvent = 0.1f;
        
        [Tooltip("Maximum intensity multiplier cap")]
        [SerializeField] private float maxIntensityMultiplier = 3f;
        
        [Header("Day-Based Scaling")]
        [Tooltip("Whether to use day-based intensity scaling (instead of event count)")]
        [SerializeField] private bool useDayBasedScaling = true;
        
        [Tooltip("Intensity multiplier increase per in-game day")]
        [SerializeField] private float dayScalingFactor = 0.05f;
        
        [Tooltip("Base days before scaling starts (grace period)")]
        [SerializeField] private float baseDaysBeforeScaling = 0f;
        
        [Header("Physics")]
        [Tooltip("Force applied to player when caught in wave (below safe elevation)")]
        [SerializeField] private float playerForceMultiplier = 50f;
        
        [Tooltip("Damage dealt to player per second while in wave")]
        [SerializeField] private float playerDamagePerSecond = 10f;
        
        [Tooltip("Damage multiplier for structures tagged 'LowStructure'")]
        [SerializeField] private float lowStructureDamageMultiplier = 2f;

        // Public properties
        public float MinInterval => minInterval;
        public float MaxInterval => maxInterval;
        public float WarningDuration => warningDuration;
        public float WarningWaterLowerAmount => warningWaterLowerAmount;
        public float WarningWaterLowerSpeed => warningWaterLowerSpeed;
        public float BaseWaveIntensity => baseWaveIntensity;
        public float WaveWaterRiseAmount => waveWaterRiseAmount;
        public float WaveWaterRiseSpeed => waveWaterRiseSpeed;
        public float WavePeakDuration => wavePeakDuration;
        public bool EnableIntensityScaling => enableIntensityScaling;
        public float IntensityIncreasePerEvent => intensityIncreasePerEvent;
        public float MaxIntensityMultiplier => maxIntensityMultiplier;
        public bool UseDayBasedScaling => useDayBasedScaling;
        public float DayScalingFactor => dayScalingFactor;
        public float BaseDaysBeforeScaling => baseDaysBeforeScaling;
        public float PlayerForceMultiplier => playerForceMultiplier;
        public float PlayerDamagePerSecond => playerDamagePerSecond;
        public float LowStructureDamageMultiplier => lowStructureDamageMultiplier;
        
        /// <summary>
        /// Gets a random interval between min and max interval.
        /// </summary>
        public float GetRandomInterval()
        {
            return Random.Range(minInterval, maxInterval);
        }
        
        /// <summary>
        /// Calculates current intensity multiplier based on event count.
        /// </summary>
        public float GetIntensityMultiplier(int eventCount)
        {
            if (!enableIntensityScaling) return 1f;
            
            float multiplier = 1f + (intensityIncreasePerEvent * eventCount);
            return Mathf.Min(multiplier, maxIntensityMultiplier);
        }
        
        /// <summary>
        /// Calculates current intensity multiplier based on in-game days.
        /// </summary>
        public float GetIntensityMultiplierByDays(float daysPassed)
        {
            if (!enableIntensityScaling || !useDayBasedScaling) return 1f;
            
            float effectiveDays = Mathf.Max(0f, daysPassed - baseDaysBeforeScaling);
            float multiplier = 1f + (dayScalingFactor * effectiveDays);
            return Mathf.Min(multiplier, maxIntensityMultiplier);
        }
    }
}
