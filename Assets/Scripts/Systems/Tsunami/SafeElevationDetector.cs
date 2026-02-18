using UnityEngine;

namespace Tsarkel.Systems.Tsunami
{
    /// <summary>
    /// Detects safe elevation levels for building placement and player safety.
    /// Uses raycasting to determine elevation and compares against safe thresholds.
    /// </summary>
    public class SafeElevationDetector : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Minimum safe elevation above sea level")]
        [SerializeField] private float safeElevationThreshold = 10f;
        
        [Tooltip("Layer mask for ground/terrain detection")]
        [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
        
        [Tooltip("Maximum raycast distance for elevation detection")]
        [SerializeField] private float maxRaycastDistance = 100f;
        
        [Header("Current Water Level")]
        [Tooltip("Current water level (set by WaterController)")]
        [SerializeField] private float currentWaterLevel = 0f;
        
        /// <summary>
        /// Current water level.
        /// </summary>
        public float CurrentWaterLevel
        {
            get => currentWaterLevel;
            set => currentWaterLevel = value;
        }
        
        /// <summary>
        /// Safe elevation threshold.
        /// </summary>
        public float SafeElevationThreshold => safeElevationThreshold;
        
        /// <summary>
        /// Checks if a given position is at a safe elevation.
        /// </summary>
        /// <param name="position">World position to check</param>
        /// <returns>True if position is above safe elevation threshold</returns>
        public bool IsElevationSafe(Vector3 position)
        {
            float elevation = GetElevationAtPosition(position);
            return elevation >= (currentWaterLevel + safeElevationThreshold);
        }
        
        /// <summary>
        /// Gets the elevation at a given world position.
        /// </summary>
        /// <param name="position">World position to check</param>
        /// <returns>Elevation (Y coordinate) at the position</returns>
        public float GetElevationAtPosition(Vector3 position)
        {
            // Raycast downward to find ground
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(position.x, position.y + 10f, position.z);
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, maxRaycastDistance, groundLayerMask))
            {
                return hit.point.y;
            }
            
            // If no ground found, return the position's Y coordinate
            return position.y;
        }
        
        /// <summary>
        /// Gets the safe elevation for a given position (water level + threshold).
        /// </summary>
        /// <param name="position">World position (for context, though not always needed)</param>
        /// <returns>Safe elevation value</returns>
        public float GetSafeElevation(Vector3 position)
        {
            return currentWaterLevel + safeElevationThreshold;
        }
        
        /// <summary>
        /// Checks if a building can be placed at the given position.
        /// </summary>
        /// <param name="position">World position to check</param>
        /// <param name="minRequiredElevation">Minimum elevation required for the building</param>
        /// <returns>True if position meets elevation requirements</returns>
        public bool CanPlaceBuildingAt(Vector3 position, float minRequiredElevation = 0f)
        {
            float elevation = GetElevationAtPosition(position);
            float requiredElevation = Mathf.Max(currentWaterLevel + safeElevationThreshold, minRequiredElevation);
            return elevation >= requiredElevation;
        }
        
        /// <summary>
        /// Gets the distance from a position to the safe elevation threshold.
        /// </summary>
        /// <param name="position">World position to check</param>
        /// <returns>Distance to safe elevation (negative if below, positive if above)</returns>
        public float GetDistanceToSafeElevation(Vector3 position)
        {
            float elevation = GetElevationAtPosition(position);
            float safeElevation = currentWaterLevel + safeElevationThreshold;
            return elevation - safeElevation;
        }
        
        /// <summary>
        /// Draws debug gizmos in the editor.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw safe elevation threshold
            Gizmos.color = Color.green;
            Vector3 safeLineStart = new Vector3(transform.position.x - 50f, currentWaterLevel + safeElevationThreshold, transform.position.z);
            Vector3 safeLineEnd = new Vector3(transform.position.x + 50f, currentWaterLevel + safeElevationThreshold, transform.position.z);
            Gizmos.DrawLine(safeLineStart, safeLineEnd);
            
            // Draw current water level
            Gizmos.color = Color.blue;
            Vector3 waterLineStart = new Vector3(transform.position.x - 50f, currentWaterLevel, transform.position.z);
            Vector3 waterLineEnd = new Vector3(transform.position.x + 50f, currentWaterLevel, transform.position.z);
            Gizmos.DrawLine(waterLineStart, waterLineEnd);
        }
    }
}
