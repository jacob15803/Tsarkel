using UnityEngine;

namespace Tsarkel.ScriptableObjects.Items
{
    /// <summary>
    /// Data definition for a weapon item.
    /// Extends ItemData with weapon-specific properties.
    /// Note: In Unity, ScriptableObjects cannot inherit from other ScriptableObjects directly.
    /// This should be used as a separate asset type, or ItemData should be modified to include weapon properties.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Tsarkel/Items/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Item data reference (for item properties)")]
        [SerializeField] private ItemData itemData;
        
        [Header("Weapon Stats")]
        [Tooltip("Base damage")]
        [SerializeField] private float damage = 20f;
        
        [Tooltip("Attack speed (attacks per second)")]
        [SerializeField] private float attackSpeed = 1f;
        
        [Tooltip("Attack range")]
        [SerializeField] private float attackRange = 2f;
        
        [Tooltip("Weapon type")]
        [SerializeField] private WeaponType weaponType = WeaponType.Melee;
        
        [Header("Durability")]
        [Tooltip("Maximum durability")]
        [SerializeField] private float maxDurability = 100f;
        
        [Tooltip("Durability loss per use")]
        [SerializeField] private float durabilityLossPerUse = 1f;

        // Public properties
        public ItemData ItemData => itemData;
        public float Damage => damage;
        public float AttackSpeed => attackSpeed;
        public float AttackRange => attackRange;
        public WeaponType Type => weaponType;
        public float MaxDurability => maxDurability;
        public float DurabilityLossPerUse => durabilityLossPerUse;
    }
    
    /// <summary>
    /// Weapon type enumeration.
    /// </summary>
    public enum WeaponType
    {
        Melee,
        Ranged,
        Tool,
        Trap
    }
}
