using System.Collections.Generic;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Crafting;
using Tsarkel.ScriptableObjects.Items;

namespace Tsarkel.Systems.Crafting
{
    /// <summary>
    /// Central manager for crafting system.
    /// Handles recipe processing and item creation.
    /// </summary>
    public class CraftingManager : MonoBehaviour
    {
        [Header("Crafting Settings")]
        [Tooltip("All available recipes")]
        [SerializeField] private CraftingRecipe[] allRecipes;
        
        private Dictionary<string, CraftingRecipe> recipeDictionary;
        
        private void Start()
        {
            // Build recipe dictionary for fast lookup
            recipeDictionary = new Dictionary<string, CraftingRecipe>();
            if (allRecipes != null)
            {
                foreach (var recipe in allRecipes)
                {
                    if (recipe != null && !string.IsNullOrEmpty(recipe.RecipeName))
                    {
                        recipeDictionary[recipe.RecipeName] = recipe;
                    }
                }
            }
        }
        
        /// <summary>
        /// Processes a crafting recipe.
        /// </summary>
        public bool ProcessCrafting(CraftingRecipe recipe, CraftingStation station)
        {
            if (recipe == null || station == null) return false;
            
            // Note: In a full implementation, this would:
            // 1. Check player inventory for ingredients
            // 2. Consume ingredients
            // 3. Create result item
            // 4. Add result to inventory
            
            // For now, just create the item GameObject
            if (recipe.ResultItem != null && recipe.ResultItem.WorldPrefab != null)
            {
                GameObject craftedItem = Instantiate(recipe.ResultItem.WorldPrefab);
                
                // Note: WeaponData would need to be checked separately if using a different system
                // For now, we'll add ItemDurability to all crafted items
                var durability = craftedItem.GetComponent<Items.ItemDurability>();
                if (durability == null)
                {
                    durability = craftedItem.AddComponent<Items.ItemDurability>();
                    durability.Initialize(100f); // Default durability
                }
                
                EventManager.Instance.InvokeItemCrafted(recipe.ResultItem, craftedItem);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets a recipe by name.
        /// </summary>
        public CraftingRecipe GetRecipe(string recipeName)
        {
            if (recipeDictionary != null && recipeDictionary.ContainsKey(recipeName))
            {
                return recipeDictionary[recipeName];
            }
            return null;
        }
        
        /// <summary>
        /// Gets all available recipes.
        /// </summary>
        public CraftingRecipe[] GetAllRecipes()
        {
            return allRecipes;
        }
    }
}
