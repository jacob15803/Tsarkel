using System.Collections.Generic;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Systems.Zone
{
    /// <summary>
    /// Central manager for zone-based world risk system.
    /// Tracks player's current zones and coordinates zone-based systems.
    /// </summary>
    public class ZoneManager : MonoBehaviour
    {
        [Header("State")]
        [Tooltip("Currently active zones (player can be in multiple zones)")]
        [SerializeField] private List<ZoneData> activeZones = new List<ZoneData>();
        
        private static ZoneManager _instance;
        
        /// <summary>
        /// Singleton instance of ZoneManager.
        /// </summary>
        public static ZoneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ZoneManager>();
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Currently active zones.
        /// </summary>
        public List<ZoneData> ActiveZones => new List<ZoneData>(activeZones);
        
        /// <summary>
        /// Primary zone (first active zone, or null if none).
        /// </summary>
        public ZoneData PrimaryZone => activeZones.Count > 0 ? activeZones[0] : null;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Adds a zone to the active zones list.
        /// </summary>
        public void EnterZone(ZoneData zoneData)
        {
            if (zoneData == null) return;
            
            if (!activeZones.Contains(zoneData))
            {
                activeZones.Add(zoneData);
                EventManager.Instance.InvokeZoneEntered(zoneData);
            }
        }
        
        /// <summary>
        /// Removes a zone from the active zones list.
        /// </summary>
        public void ExitZone(ZoneData zoneData)
        {
            if (zoneData == null) return;
            
            if (activeZones.Contains(zoneData))
            {
                activeZones.Remove(zoneData);
                EventManager.Instance.InvokeZoneExited(zoneData);
            }
        }
        
        /// <summary>
        /// Checks if player is in a specific zone.
        /// </summary>
        public bool IsInZone(ZoneData zoneData)
        {
            return activeZones.Contains(zoneData);
        }
        
        /// <summary>
        /// Gets the combined predator spawn multiplier from all active zones.
        /// </summary>
        public float GetCombinedPredatorSpawnMultiplier()
        {
            if (activeZones.Count == 0) return 1f;
            
            float multiplier = 1f;
            foreach (var zone in activeZones)
            {
                multiplier *= zone.PredatorSpawnMultiplier;
            }
            
            return multiplier;
        }
        
        /// <summary>
        /// Gets the combined resource spawn multiplier from all active zones.
        /// </summary>
        public float GetCombinedResourceSpawnMultiplier()
        {
            if (activeZones.Count == 0) return 1f;
            
            float multiplier = 1f;
            foreach (var zone in activeZones)
            {
                multiplier *= zone.ResourceSpawnMultiplier;
            }
            
            return multiplier;
        }
        
        /// <summary>
        /// Gets the highest risk level from all active zones.
        /// </summary>
        public float GetMaxRiskLevel()
        {
            if (activeZones.Count == 0) return 0f;
            
            float maxRisk = 0f;
            foreach (var zone in activeZones)
            {
                maxRisk = Mathf.Max(maxRisk, zone.RiskLevel);
            }
            
            return maxRisk;
        }
        
        /// <summary>
        /// Checks if player is in any tribal territory.
        /// </summary>
        public bool IsInTribalTerritory()
        {
            foreach (var zone in activeZones)
            {
                if (zone.IsTribalTerritory)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
