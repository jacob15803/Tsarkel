using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Systems.Items
{
    /// <summary>
    /// Durability system for crafted items.
    /// Handles item degradation and breaking.
    /// </summary>
    public class ItemDurability : MonoBehaviour
    {
        [Header("Durability State")]
        [Tooltip("Current durability")]
        [SerializeField] private float currentDurability;
        
        [Tooltip("Maximum durability")]
        [SerializeField] private float maxDurability = 100f;
        
        private bool isInitialized = false;
        
        /// <summary>
        /// Current durability.
        /// </summary>
        public float CurrentDurability => currentDurability;
        
        /// <summary>
        /// Maximum durability.
        /// </summary>
        public float MaxDurability => maxDurability;
        
        /// <summary>
        /// Durability as a percentage (0-1).
        /// </summary>
        public float DurabilityPercentage => maxDurability > 0 ? currentDurability / maxDurability : 0f;
        
        /// <summary>
        /// Whether the item is broken.
        /// </summary>
        public bool IsBroken => currentDurability <= 0f;
        
        /// <summary>
        /// Initializes the durability system.
        /// </summary>
        public void Initialize(float maxDurabilityValue)
        {
            maxDurability = maxDurabilityValue;
            currentDurability = maxDurability;
            isInitialized = true;
        }
        
        /// <summary>
        /// Reduces durability by the specified amount.
        /// </summary>
        public void ReduceDurability(float amount)
        {
            if (!isInitialized) return;
            
            float oldDurability = currentDurability;
            currentDurability = Mathf.Max(0f, currentDurability - amount);
            
            // Invoke durability changed event
            var weaponComponent = GetComponent<WeaponComponent>();
            if (weaponComponent != null)
            {
                EventManager.Instance.InvokeWeaponDurabilityChanged(gameObject, currentDurability, maxDurability);
            }
            
            // Check if broken
            if (currentDurability <= 0f && oldDurability > 0f)
            {
                OnItemBroke();
            }
        }
        
        /// <summary>
        /// Repairs the item by the specified amount.
        /// </summary>
        public void Repair(float amount)
        {
            if (!isInitialized) return;
            
            currentDurability = Mathf.Min(maxDurability, currentDurability + amount);
            
            var weaponComponent = GetComponent<WeaponComponent>();
            if (weaponComponent != null)
            {
                EventManager.Instance.InvokeWeaponDurabilityChanged(gameObject, currentDurability, maxDurability);
            }
        }
        
        /// <summary>
        /// Handles item breaking.
        /// </summary>
        private void OnItemBroke()
        {
            EventManager.Instance.InvokeWeaponBroke(gameObject);
            
            // Item is broken but not destroyed (can be repaired)
            Debug.Log($"{gameObject.name} has broken!");
        }
    }
}
