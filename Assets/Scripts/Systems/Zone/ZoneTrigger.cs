using UnityEngine;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Systems.Zone
{
    /// <summary>
    /// Trigger component for zone boundaries.
    /// Detects when player enters/exits a zone.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ZoneTrigger : MonoBehaviour
    {
        [Header("Zone Configuration")]
        [Tooltip("Zone data asset")]
        [SerializeField] private ZoneData zoneData;
        
        [Header("Settings")]
        [Tooltip("Whether this trigger is active")]
        [SerializeField] private bool isActive = true;
        
        private Collider triggerCollider;
        private ZoneManager zoneManager;
        
        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
            
            if (zoneManager == null)
            {
                zoneManager = ZoneManager.Instance;
            }
        }
        
        private void Start()
        {
            if (zoneData == null)
            {
                Debug.LogWarning($"ZoneTrigger on {gameObject.name}: ZoneData is not assigned!");
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || zoneData == null) return;
            
            // Check if it's the player
            if (other.CompareTag("Player"))
            {
                if (zoneManager != null)
                {
                    zoneManager.EnterZone(zoneData);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!isActive || zoneData == null) return;
            
            // Check if it's the player
            if (other.CompareTag("Player"))
            {
                if (zoneManager != null)
                {
                    zoneManager.ExitZone(zoneData);
                }
            }
        }
        
        /// <summary>
        /// Sets the zone data for this trigger.
        /// </summary>
        public void SetZoneData(ZoneData data)
        {
            zoneData = data;
        }
        
        /// <summary>
        /// Sets whether this trigger is active.
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
        }
    }
}
