using UnityEngine;

namespace Tsarkel.ScriptableObjects.Zone
{
    /// <summary>
    /// Configuration data for a world zone.
    /// Defines risk levels, spawn rates, and resource availability.
    /// </summary>
    [CreateAssetMenu(fileName = "ZoneData", menuName = "Tsarkel/Zone/Zone Data")]
    public class ZoneData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Zone name")]
        [SerializeField] private string zoneName = "New Zone";
        
        [Tooltip("Zone description")]
        [TextArea(3, 5)]
        [SerializeField] private string description = "";
        
        [Header("Risk Settings")]
        [Tooltip("Risk level (0-1, where 1 = maximum risk)")]
        [SerializeField] private float riskLevel = 0.5f;
        
        [Tooltip("Whether this zone is a tribal territory")]
        [SerializeField] private bool isTribalTerritory = false;
        
        [Header("Spawn Multipliers")]
        [Tooltip("Predator spawn rate multiplier")]
        [SerializeField] private float predatorSpawnMultiplier = 1f;
        
        [Tooltip("Resource spawn rate multiplier")]
        [SerializeField] private float resourceSpawnMultiplier = 1f;
        
        [Tooltip("Rare resource spawn rate multiplier")]
        [SerializeField] private float rareResourceSpawnMultiplier = 1f;
        
        [Header("Environmental Modifiers")]
        [Tooltip("Tsunami vulnerability (0-1, where 1 = highly vulnerable)")]
        [SerializeField] private float tsunamiVulnerability = 0.5f;
        
        [Tooltip("Visibility modifier (affects detection ranges)")]
        [SerializeField] private float visibilityModifier = 1f;
        
        [Header("Resource Availability")]
        [Tooltip("Common resources available in this zone")]
        [SerializeField] private string[] commonResources;
        
        [Tooltip("Rare resources available in this zone")]
        [SerializeField] private string[] rareResources;
        
        [Tooltip("Unique resources only found in this zone")]
        [SerializeField] private string[] uniqueResources;

        // Public properties
        public string ZoneName => zoneName;
        public string Description => description;
        public float RiskLevel => riskLevel;
        public bool IsTribalTerritory => isTribalTerritory;
        public float PredatorSpawnMultiplier => predatorSpawnMultiplier;
        public float ResourceSpawnMultiplier => resourceSpawnMultiplier;
        public float RareResourceSpawnMultiplier => rareResourceSpawnMultiplier;
        public float TsunamiVulnerability => tsunamiVulnerability;
        public float VisibilityModifier => visibilityModifier;
        public string[] CommonResources => commonResources;
        public string[] RareResources => rareResources;
        public string[] UniqueResources => uniqueResources;
    }
}
