using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.Systems.Zone;
using Tsarkel.ScriptableObjects.Tribal;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Systems.Tribal
{
    /// <summary>
    /// Central manager for tribal presence system.
    /// Coordinates hostility, ambushes, and territory tracking.
    /// </summary>
    public class TribalManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tribal configuration asset")]
        [SerializeField] private TribalConfig config;
        
        [Header("Components")]
        [Tooltip("Tribal hostility meter")]
        [SerializeField] private TribalHostilityMeter hostilityMeter;
        
        [Tooltip("Tribal observer")]
        [SerializeField] private TribalObserver observer;
        
        [Tooltip("Tribal ambush system")]
        [SerializeField] private TribalAmbush ambush;
        
        [Header("Dependencies")]
        [Tooltip("Zone manager reference")]
        [SerializeField] private ZoneManager zoneManager;
        
        [Header("Update Settings")]
        [Tooltip("Ambush check interval (seconds)")]
        [SerializeField] private float ambushCheckInterval = 5f;
        
        private float lastAmbushCheck = 0f;
        
        private void Start()
        {
            if (zoneManager == null)
            {
                zoneManager = ZoneManager.Instance;
            }
            
            if (hostilityMeter == null)
            {
                hostilityMeter = FindObjectOfType<TribalHostilityMeter>();
            }
            
            if (observer == null)
            {
                observer = FindObjectOfType<TribalObserver>();
            }
            
            if (ambush == null)
            {
                ambush = FindObjectOfType<TribalAmbush>();
            }
            
            // Subscribe to zone events
            EventManager.Instance.OnZoneEntered += HandleZoneEntered;
            EventManager.Instance.OnZoneExited += HandleZoneExited;
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.OnZoneEntered -= HandleZoneEntered;
            EventManager.Instance.OnZoneExited -= HandleZoneExited;
        }
        
        private void Update()
        {
            // Update territory status
            if (hostilityMeter != null && zoneManager != null)
            {
                bool inTerritory = zoneManager.IsInTribalTerritory();
                hostilityMeter.SetInTerritory(inTerritory);
            }
            
            // Check for ambushes
            if (Time.time - lastAmbushCheck >= ambushCheckInterval)
            {
                lastAmbushCheck = Time.time;
                CheckAmbush();
            }
        }
        
        /// <summary>
        /// Handles zone entered event.
        /// </summary>
        private void HandleZoneEntered(ZoneData zoneData)
        {
            if (zoneData != null && zoneData.IsTribalTerritory)
            {
                // Player entered tribal territory
                if (hostilityMeter != null)
                {
                    hostilityMeter.SetInTerritory(true);
                }
            }
        }
        
        /// <summary>
        /// Handles zone exited event.
        /// </summary>
        private void HandleZoneExited(ZoneData zoneData)
        {
            if (zoneData != null && zoneData.IsTribalTerritory)
            {
                // Check if still in any tribal territory
                if (zoneManager != null && !zoneManager.IsInTribalTerritory())
                {
                    if (hostilityMeter != null)
                    {
                        hostilityMeter.SetInTerritory(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if an ambush should be triggered.
        /// </summary>
        private void CheckAmbush()
        {
            if (ambush != null && ambush.ShouldTriggerAmbush())
            {
                ambush.TriggerAmbush();
            }
        }
    }
}
