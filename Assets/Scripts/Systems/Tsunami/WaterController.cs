using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Systems.Tsunami
{
    /// <summary>
    /// Manages water level transitions during tsunami events.
    /// Handles smooth water level changes for warning and wave phases.
    /// </summary>
    public class WaterController : MonoBehaviour
    {
        [Header("Water Plane")]
        [Tooltip("Transform representing the water plane (will be moved up/down)")]
        [SerializeField] private Transform waterPlane;
        
        [Header("Initial Settings")]
        [Tooltip("Starting water level")]
        [SerializeField] private float initialWaterLevel = 0f;
        
        private float currentWaterLevel;
        private float targetWaterLevel;
        private float waterChangeSpeed;
        private bool isChanging;
        
        /// <summary>
        /// Current water level.
        /// </summary>
        public float CurrentWaterLevel => currentWaterLevel;
        
        private void Start()
        {
            currentWaterLevel = initialWaterLevel;
            targetWaterLevel = initialWaterLevel;
            
            // Update water plane position
            UpdateWaterPlanePosition();
            
            // Update elevation detector if available
            UpdateElevationDetector();
        }
        
        private void Update()
        {
            if (isChanging)
            {
                UpdateWaterLevel();
            }
        }
        
        /// <summary>
        /// Sets the target water level and speed of transition.
        /// </summary>
        /// <param name="targetLevel">Target water level</param>
        /// <param name="changeSpeed">Speed of water level change (units per second)</param>
        public void SetTargetWaterLevel(float targetLevel, float changeSpeed)
        {
            targetWaterLevel = targetLevel;
            waterChangeSpeed = changeSpeed;
            isChanging = true;
        }
        
        /// <summary>
        /// Updates water level smoothly towards target.
        /// </summary>
        private void UpdateWaterLevel()
        {
            if (Mathf.Approximately(currentWaterLevel, targetWaterLevel))
            {
                currentWaterLevel = targetWaterLevel;
                isChanging = false;
            }
            else
            {
                float direction = targetWaterLevel > currentWaterLevel ? 1f : -1f;
                float change = waterChangeSpeed * Time.deltaTime * direction;
                
                if (direction > 0f)
                {
                    currentWaterLevel = Mathf.Min(currentWaterLevel + change, targetWaterLevel);
                }
                else
                {
                    currentWaterLevel = Mathf.Max(currentWaterLevel + change, targetWaterLevel);
                }
            }
            
            UpdateWaterPlanePosition();
            UpdateElevationDetector();
            
            // Invoke water level changed event
            EventManager.Instance.InvokeWaterLevelChanged(currentWaterLevel);
        }
        
        /// <summary>
        /// Updates the visual water plane position.
        /// </summary>
        private void UpdateWaterPlanePosition()
        {
            if (waterPlane != null)
            {
                Vector3 position = waterPlane.position;
                position.y = currentWaterLevel;
                waterPlane.position = position;
            }
        }
        
        /// <summary>
        /// Updates the elevation detector with current water level.
        /// </summary>
        private void UpdateElevationDetector()
        {
            SafeElevationDetector detector = FindObjectOfType<SafeElevationDetector>();
            if (detector != null)
            {
                detector.CurrentWaterLevel = currentWaterLevel;
            }
        }
        
        /// <summary>
        /// Sets water level immediately (no transition).
        /// </summary>
        public void SetWaterLevelImmediate(float level)
        {
            currentWaterLevel = level;
            targetWaterLevel = level;
            isChanging = false;
            
            UpdateWaterPlanePosition();
            UpdateElevationDetector();
            EventManager.Instance.InvokeWaterLevelChanged(currentWaterLevel);
        }
        
        /// <summary>
        /// Resets water level to initial value.
        /// </summary>
        public void ResetWaterLevel()
        {
            SetTargetWaterLevel(initialWaterLevel, 1f);
        }
    }
}
