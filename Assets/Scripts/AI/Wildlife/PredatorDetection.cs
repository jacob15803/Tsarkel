using UnityEngine;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Detection system for predators.
    /// Handles sight and hearing detection of the player.
    /// </summary>
    public class PredatorDetection : MonoBehaviour
    {
        [Header("Detection Settings")]
        [Tooltip("Sight detection radius")]
        [SerializeField] private float sightRadius = 15f;
        
        [Tooltip("Sight detection angle (degrees)")]
        [SerializeField] private float sightAngle = 90f;
        
        [Tooltip("Hearing detection radius")]
        [SerializeField] private float hearingRadius = 10f;
        
        [Tooltip("Layer mask for obstacles (blocks sight)")]
        [SerializeField] private LayerMask obstacleLayerMask = 1;
        
        private Transform playerTransform;
        private bool hasDetectedPlayer = false;
        
        /// <summary>
        /// Whether the predator has detected the player.
        /// </summary>
        public bool HasDetectedPlayer => hasDetectedPlayer;
        
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        /// <summary>
        /// Checks if player is within sight range and angle.
        /// </summary>
        public bool CanSeePlayer()
        {
            if (playerTransform == null) return false;
            
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float distance = directionToPlayer.magnitude;
            
            // Check distance
            if (distance > sightRadius) return false;
            
            // Check angle (forward-facing cone)
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle > sightAngle * 0.5f) return false;
            
            // Check for obstacles (raycast)
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, distance, obstacleLayerMask))
            {
                // If we hit something that's not the player, we can't see them
                if (hit.collider.transform != playerTransform)
                {
                    return false;
                }
            }
            
            hasDetectedPlayer = true;
            return true;
        }
        
        /// <summary>
        /// Checks if player is within hearing range.
        /// </summary>
        public bool CanHearPlayer(float playerNoiseLevel = 1f)
        {
            if (playerTransform == null) return false;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float effectiveHearingRadius = hearingRadius * playerNoiseLevel;
            
            return distance <= effectiveHearingRadius;
        }
        
        /// <summary>
        /// Gets the direction to the player.
        /// </summary>
        public Vector3 GetDirectionToPlayer()
        {
            if (playerTransform == null) return Vector3.zero;
            
            return (playerTransform.position - transform.position).normalized;
        }
        
        /// <summary>
        /// Gets the distance to the player.
        /// </summary>
        public float GetDistanceToPlayer()
        {
            if (playerTransform == null) return float.MaxValue;
            
            return Vector3.Distance(transform.position, playerTransform.position);
        }
        
        /// <summary>
        /// Resets detection state.
        /// </summary>
        public void ResetDetection()
        {
            hasDetectedPlayer = false;
        }
    }
}
