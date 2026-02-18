using UnityEngine;

namespace Tsarkel.ScriptableObjects.Buildings
{
    /// <summary>
    /// Data definition for a buildable structure.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Tsarkel/Buildings/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Display name of the building")]
        [SerializeField] private string buildingName = "New Building";
        
        [Tooltip("Description of the building")]
        [TextArea(3, 5)]
        [SerializeField] private string description = "";
        
        [Tooltip("Icon sprite for UI display")]
        [SerializeField] private Sprite icon;
        
        [Header("Prefab")]
        [Tooltip("Prefab to instantiate when placing this building")]
        [SerializeField] private GameObject prefab;
        
        [Header("Resources")]
        [Tooltip("Required resources to build (item ID and quantity pairs)")]
        [SerializeField] private ResourceRequirement[] requiredResources;
        
        [Header("Durability")]
        [Tooltip("Maximum health/durability of this structure")]
        [SerializeField] private float maxDurability = 100f;
        
        [Tooltip("Damage resistance multiplier (1.0 = normal, 0.5 = half damage, 2.0 = double damage)")]
        [SerializeField] private float damageResistance = 1f;
        
        [Header("Placement Rules")]
        [Tooltip("Minimum elevation required to place this building")]
        [SerializeField] private float minElevation = 0f;
        
        [Tooltip("Whether this building can be placed on water")]
        [SerializeField] private bool canPlaceOnWater = false;
        
        [Tooltip("Whether this building requires flat ground")]
        [SerializeField] private bool requiresFlatGround = false;
        
        [Tooltip("Maximum ground angle for placement (in degrees)")]
        [SerializeField] private float maxGroundAngle = 45f;
        
        [Tooltip("Snap to grid size (0 = no grid snapping)")]
        [SerializeField] private float gridSnapSize = 0f;
        
        [Header("Tags")]
        [Tooltip("Tag to apply to this building (e.g., 'LowStructure' for tsunami destruction)")]
        [SerializeField] private string buildingTag = "Untagged";

        // Public properties
        public string BuildingName => buildingName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
        public ResourceRequirement[] RequiredResources => requiredResources;
        public float MaxDurability => maxDurability;
        public float DamageResistance => damageResistance;
        public float MinElevation => minElevation;
        public bool CanPlaceOnWater => canPlaceOnWater;
        public bool RequiresFlatGround => requiresFlatGround;
        public float MaxGroundAngle => maxGroundAngle;
        public float GridSnapSize => gridSnapSize;
        public string BuildingTag => buildingTag;
        
        /// <summary>
        /// Checks if the player has enough resources to build this structure.
        /// </summary>
        public bool CanAfford(System.Collections.Generic.Dictionary<string, int> playerResources)
        {
            if (requiredResources == null || requiredResources.Length == 0)
                return true;
            
            foreach (var requirement in requiredResources)
            {
                if (!playerResources.ContainsKey(requirement.itemId) || 
                    playerResources[requirement.itemId] < requirement.quantity)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// Represents a resource requirement for building.
    /// </summary>
    [System.Serializable]
    public class ResourceRequirement
    {
        [Tooltip("Item ID of the required resource")]
        public string itemId;
        
        [Tooltip("Quantity required")]
        public int quantity;
    }
}
