using UnityEngine;
using Tsarkel.Systems.Zone;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Systems.Tribal
{
    /// <summary>
    /// Component that marks a zone as tribal territory.
    /// Integrates with zone system to track player presence.
    /// </summary>
    [RequireComponent(typeof(ZoneTrigger))]
    public class TribalTerritory : MonoBehaviour
    {
        [Header("Territory Settings")]
        [Tooltip("Zone data for this territory")]
        [SerializeField] private ZoneData zoneData;
        
        private ZoneTrigger zoneTrigger;
        private TribalManager tribalManager;
        
        private void Awake()
        {
            zoneTrigger = GetComponent<ZoneTrigger>();
            
            if (tribalManager == null)
            {
                tribalManager = FindObjectOfType<TribalManager>();
            }
        }
        
        private void Start()
        {
            // Ensure zone data is set
            if (zoneTrigger != null && zoneData != null)
            {
                zoneTrigger.SetZoneData(zoneData);
            }
        }
        
        /// <summary>
        /// Gets the zone data for this territory.
        /// </summary>
        public ZoneData GetZoneData()
        {
            return zoneData;
        }
    }
}
