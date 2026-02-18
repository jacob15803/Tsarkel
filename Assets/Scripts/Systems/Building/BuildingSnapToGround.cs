using UnityEngine;

namespace Tsarkel.Systems.Building
{
    /// <summary>
    /// Component attached to building prefabs to handle ground snapping on placement.
    /// </summary>
    public class BuildingSnapToGround : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Layer mask for ground detection")]
        [SerializeField] private LayerMask groundLayerMask = 1;
        
        [Tooltip("Maximum raycast distance")]
        [SerializeField] private float maxRaycastDistance = 10f;
        
        [Tooltip("Offset from ground (to prevent clipping)")]
        [SerializeField] private float groundOffset = 0.1f;
        
        /// <summary>
        /// Snaps the building to the ground below it.
        /// </summary>
        public void SnapToGround()
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 5f;
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, maxRaycastDistance, groundLayerMask))
            {
                Vector3 newPosition = hit.point;
                newPosition.y += groundOffset;
                transform.position = newPosition;
            }
        }
        
        /// <summary>
        /// Snaps to ground on start (for placed buildings).
        /// </summary>
        private void Start()
        {
            SnapToGround();
        }
    }
}
