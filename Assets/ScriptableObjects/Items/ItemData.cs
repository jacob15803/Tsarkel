using UnityEngine;

namespace Tsarkel.ScriptableObjects.Items
{
    /// <summary>
    /// Enumeration of item types in the game.
    /// </summary>
    public enum ItemType
    {
        Resource,
        Tool,
        Food,
        Water,
        Building,
        Misc
    }
    
    /// <summary>
    /// Data definition for an item in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Tsarkel/Items/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Unique identifier for this item")]
        [SerializeField] private string itemId = "";
        
        [Tooltip("Display name of the item")]
        [SerializeField] private string itemName = "New Item";
        
        [Tooltip("Description of the item")]
        [TextArea(3, 5)]
        [SerializeField] private string description = "";
        
        [Header("Visual")]
        [Tooltip("Icon sprite for UI display")]
        [SerializeField] private Sprite icon;
        
        [Tooltip("3D model prefab (optional, for world representation)")]
        [SerializeField] private GameObject worldPrefab;
        
        [Header("Properties")]
        [Tooltip("Type of item")]
        [SerializeField] private ItemType itemType = ItemType.Resource;
        
        [Tooltip("Maximum stack size (1 = non-stackable)")]
        [SerializeField] private int maxStackSize = 1;
        
        [Tooltip("Whether this item can be consumed")]
        [SerializeField] private bool consumable = false;
        
        [Header("Consumption Effects")]
        [Tooltip("Health restored when consumed (if consumable)")]
        [SerializeField] private float healthRestore = 0f;
        
        [Tooltip("Hunger restored when consumed (if consumable)")]
        [SerializeField] private float hungerRestore = 0f;
        
        [Tooltip("Hydration restored when consumed (if consumable)")]
        [SerializeField] private float hydrationRestore = 0f;
        
        [Tooltip("Stamina restored when consumed (if consumable)")]
        [SerializeField] private float staminaRestore = 0f;

        // Public properties
        public string ItemId => itemId;
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject WorldPrefab => worldPrefab;
        public ItemType Type => itemType;
        public int MaxStackSize => maxStackSize;
        public bool Consumable => consumable;
        public float HealthRestore => healthRestore;
        public float HungerRestore => hungerRestore;
        public float HydrationRestore => hydrationRestore;
        public float StaminaRestore => staminaRestore;
        
        /// <summary>
        /// Validates that the item ID is set (called in editor).
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(itemId))
            {
                itemId = itemName.Replace(" ", "").ToLower();
            }
        }
    }
}
