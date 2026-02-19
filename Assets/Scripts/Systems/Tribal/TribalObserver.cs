using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.Systems.Zone;
using Tsarkel.ScriptableObjects.Tribal;

namespace Tsarkel.Systems.Tribal
{
    /// <summary>
    /// Observes player actions and increases hostility accordingly.
    /// Listens to game events to track player behavior.
    /// </summary>
    public class TribalObserver : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tribal configuration asset")]
        [SerializeField] private TribalConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Tribal hostility meter reference")]
        [SerializeField] private TribalHostilityMeter hostilityMeter;
        
        private void OnEnable()
        {
            EventManager.Instance.OnBuildingPlaced += HandleBuildingPlaced;
            EventManager.Instance.OnItemCrafted += HandleItemCrafted;
            EventManager.Instance.OnPredatorAttackedPlayer += HandlePredatorAttacked;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventManager.Instance.OnItemCrafted -= HandleItemCrafted;
            EventManager.Instance.OnPredatorAttackedPlayer -= HandlePredatorAttacked;
        }
        
        private void Start()
        {
            if (hostilityMeter == null)
            {
                hostilityMeter = FindObjectOfType<TribalHostilityMeter>();
            }
        }
        
        /// <summary>
        /// Handles building placed event.
        /// </summary>
        private void HandleBuildingPlaced(GameObject building, ScriptableObjects.Buildings.BuildingData data)
        {
            if (hostilityMeter == null || config == null) return;
            
            // Check if player is in tribal territory
            var zoneManager = ZoneManager.Instance;
            if (zoneManager != null && zoneManager.IsInTribalTerritory())
            {
                hostilityMeter.IncreaseHostility(config.BuildingHostilityIncrease);
                EventManager.Instance.InvokeTribalActionPerformed("Building", config.BuildingHostilityIncrease);
            }
        }
        
        /// <summary>
        /// Handles item crafted event.
        /// </summary>
        private void HandleItemCrafted(ScriptableObjects.Items.ItemData itemData, GameObject craftedObject)
        {
            if (hostilityMeter == null || config == null) return;
            
            // Check if it's a weapon (would need to check item type or weapon component)
            var weaponComponent = craftedObject.GetComponent<Items.WeaponComponent>();
            if (weaponComponent != null)
            {
                // Check if player is in tribal territory
                var zoneManager = ZoneManager.Instance;
                if (zoneManager != null && zoneManager.IsInTribalTerritory())
                {
                    hostilityMeter.IncreaseHostility(config.WeaponCraftingHostilityIncrease);
                    EventManager.Instance.InvokeTribalActionPerformed("WeaponCrafting", config.WeaponCraftingHostilityIncrease);
                }
            }
        }
        
        /// <summary>
        /// Handles predator attack event (player attacking wildlife).
        /// </summary>
        private void HandlePredatorAttacked(GameObject predator, float damageAmount)
        {
            if (hostilityMeter == null || config == null) return;
            
            // Check if attack happened near tribal territory
            var zoneManager = ZoneManager.Instance;
            if (zoneManager != null && zoneManager.IsInTribalTerritory())
            {
                hostilityMeter.IncreaseHostility(config.AggressiveActionHostilityIncrease);
                EventManager.Instance.InvokeTribalActionPerformed("AggressiveAction", config.AggressiveActionHostilityIncrease);
            }
        }
        
        /// <summary>
        /// Manually triggers hostility increase for resource gathering.
        /// </summary>
        public void OnResourceGathered()
        {
            if (hostilityMeter == null || config == null) return;
            
            var zoneManager = ZoneManager.Instance;
            if (zoneManager != null && zoneManager.IsInTribalTerritory())
            {
                hostilityMeter.IncreaseHostility(config.ResourceGatheringHostilityIncrease);
                EventManager.Instance.InvokeTribalActionPerformed("ResourceGathering", config.ResourceGatheringHostilityIncrease);
            }
        }
    }
}
