using UnityEngine;
using Tsarkel.ScriptableObjects.Items;

namespace Tsarkel.ScriptableObjects.Crafting
{
    /// <summary>
    /// Recipe definition for crafting items.
    /// Defines required ingredients and resulting item.
    /// </summary>
    [CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Tsarkel/Crafting/Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("Recipe Info")]
        [Tooltip("Recipe name")]
        [SerializeField] private string recipeName = "New Recipe";
        
        [Tooltip("Recipe description")]
        [TextArea(2, 4)]
        [SerializeField] private string description = "";
        
        [Header("Result")]
        [Tooltip("Item produced by this recipe")]
        [SerializeField] private ItemData resultItem;
        
        [Tooltip("Quantity of result item produced")]
        [SerializeField] private int resultQuantity = 1;
        
        [Header("Ingredients")]
        [Tooltip("Required ingredients (item ID and quantity)")]
        [SerializeField] private IngredientRequirement[] ingredients;
        
        [Header("Crafting Station")]
        [Tooltip("Required crafting station type (empty = any station)")]
        [SerializeField] private string requiredStationType = "";
        
        [Tooltip("Crafting time (seconds)")]
        [SerializeField] private float craftingTime = 1f;

        // Public properties
        public string RecipeName => recipeName;
        public string Description => description;
        public ItemData ResultItem => resultItem;
        public int ResultQuantity => resultQuantity;
        public IngredientRequirement[] Ingredients => ingredients;
        public string RequiredStationType => requiredStationType;
        public float CraftingTime => craftingTime;
        
        /// <summary>
        /// Checks if the player has all required ingredients.
        /// </summary>
        public bool CanCraft(System.Collections.Generic.Dictionary<string, int> inventory)
        {
            if (ingredients == null || ingredients.Length == 0) return true;
            
            foreach (var ingredient in ingredients)
            {
                if (!inventory.ContainsKey(ingredient.itemId) || 
                    inventory[ingredient.itemId] < ingredient.quantity)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// Represents an ingredient requirement for crafting.
    /// </summary>
    [System.Serializable]
    public class IngredientRequirement
    {
        [Tooltip("Item ID of the required ingredient")]
        public string itemId;
        
        [Tooltip("Quantity required")]
        public int quantity;
    }
}
