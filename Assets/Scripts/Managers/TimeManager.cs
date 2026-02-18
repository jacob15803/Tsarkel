using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Managers
{
    /// <summary>
    /// Manages game time, day/night cycle, and time-based events.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        [Header("Time Settings")]
        [Tooltip("Time scale (1.0 = real time, 60.0 = 1 minute per second)")]
        [SerializeField] private float timeScale = 60f;
        
        [Tooltip("Length of a full day in seconds (at timeScale = 1)")]
        [SerializeField] private float dayLength = 86400f; // 24 hours in seconds
        
        [Tooltip("Starting time of day (0-1, where 0 = midnight, 0.5 = noon)")]
        [SerializeField] private float startingTimeOfDay = 0.25f; // 6 AM
        
        [Header("Time Events")]
        [Tooltip("Whether to invoke time change events")]
        [SerializeField] private bool invokeTimeEvents = true;
        
        [Tooltip("Time change event frequency (seconds)")]
        [SerializeField] private float timeEventInterval = 1f;
        
        private float currentTime = 0f;
        private float lastTimeEvent = 0f;
        
        /// <summary>
        /// Current time of day (0-1).
        /// </summary>
        public float TimeOfDay => currentTime;
        
        /// <summary>
        /// Current hour (0-24).
        /// </summary>
        public float CurrentHour => TimeOfDay * 24f;
        
        /// <summary>
        /// Whether it is currently day time.
        /// </summary>
        public bool IsDay => TimeOfDay > 0.25f && TimeOfDay < 0.75f;
        
        /// <summary>
        /// Whether it is currently night time.
        /// </summary>
        public bool IsNight => !IsDay;
        
        private void Start()
        {
            currentTime = startingTimeOfDay;
        }
        
        private void Update()
        {
            // Update time
            float deltaTime = Time.deltaTime * timeScale;
            currentTime += deltaTime / dayLength;
            
            // Wrap time around
            if (currentTime >= 1f)
            {
                currentTime -= 1f;
            }
            
            // Invoke time change events
            if (invokeTimeEvents && Time.time - lastTimeEvent >= timeEventInterval)
            {
                lastTimeEvent = Time.time;
                EventManager.Instance.InvokeGameTimeChanged(currentTime);
            }
        }
        
        /// <summary>
        /// Sets the time of day.
        /// </summary>
        public void SetTimeOfDay(float time)
        {
            currentTime = Mathf.Clamp01(time);
        }
        
        /// <summary>
        /// Sets the time scale.
        /// </summary>
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(0f, scale);
        }
    }
}
