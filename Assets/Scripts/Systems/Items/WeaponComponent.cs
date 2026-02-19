using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Items;

namespace Tsarkel.Systems.Items
{
    /// <summary>
    /// Component for weapon items.
    /// Handles weapon stats and combat functionality.
    /// </summary>
    public class WeaponComponent : MonoBehaviour
    {
        [Header("Weapon Data")]
        [Tooltip("Weapon data asset")]
        [SerializeField] private WeaponData weaponData;
        
        [Header("Durability")]
        [Tooltip("Item durability component")]
        [SerializeField] private ItemDurability durability;
        
        private float lastAttackTime = 0f;
        
        /// <summary>
        /// Weapon data.
        /// </summary>
        public WeaponData WeaponData => weaponData;
        
        /// <summary>
        /// Current durability.
        /// </summary>
        public float CurrentDurability => durability != null ? durability.CurrentDurability : 0f;
        
        private void Awake()
        {
            if (durability == null)
            {
                durability = GetComponent<ItemDurability>();
            }
        }
        
        /// <summary>
        /// Initializes the weapon with weapon data.
        /// </summary>
        public void Initialize(WeaponData data)
        {
            weaponData = data;
            
            if (durability == null)
            {
                durability = GetComponent<ItemDurability>();
                if (durability == null)
                {
                    durability = gameObject.AddComponent<ItemDurability>();
                }
            }
            
            if (durability != null && data != null)
            {
                durability.Initialize(data.MaxDurability);
            }
        }
        
        /// <summary>
        /// Attempts to attack with this weapon.
        /// </summary>
        public bool Attack(GameObject target)
        {
            if (weaponData == null) return false;
            
            // Check cooldown
            if (Time.time - lastAttackTime < 1f / weaponData.AttackSpeed)
            {
                return false;
            }
            
            // Check durability
            if (durability != null && durability.IsBroken)
            {
                return false;
            }
            
            // Check range
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > weaponData.AttackRange)
            {
                return false;
            }
            
            // Perform attack
            lastAttackTime = Time.time;
            
            // Apply damage to target
            var playerStats = target.GetComponent<Player.PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(weaponData.Damage, weaponData.ItemName);
            }
            
            // Reduce durability
            if (durability != null && weaponData != null)
            {
                durability.ReduceDurability(weaponData.DurabilityLossPerUse);
            }
            
            return true;
        }
    }
}
