using UnityEngine;
using Tsarkel.ScriptableObjects.Crafting;

namespace Tsarkel.Systems.Crafting
{
    /// <summary>
    /// Crafting station component.
    /// Handles recipe processing and item creation.
    /// </summary>
    public class CraftingStation : MonoBehaviour
    {
        [Header("Station Settings")]
        [Tooltip("Station type identifier")]
        [SerializeField] private string stationType = "Basic";
        
        [Tooltip("Available recipes at this station")]
        [SerializeField] private CraftingRecipe[] availableRecipes;
        
        [Header("UI")]
        [Tooltip("Crafting UI panel (optional)")]
        [SerializeField] private GameObject craftingUIPanel;
        
        private bool isPlayerNearby = false;
        private CraftingManager craftingManager;
        
        /// <summary>
        /// Station type identifier.
        /// </summary>
        public string StationType => stationType;
        
        /// <summary>
        /// Available recipes at this station.
        /// </summary>
        public CraftingRecipe[] AvailableRecipes => availableRecipes;
        
        private void Start()
        {
            if (craftingManager == null)
            {
                craftingManager = FindObjectOfType<CraftingManager>();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = true;
                if (craftingUIPanel != null)
                {
                    craftingUIPanel.SetActive(true);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                if (craftingUIPanel != null)
                {
                    craftingUIPanel.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Attempts to craft an item using the specified recipe.
        /// </summary>
        public bool CraftItem(CraftingRecipe recipe, System.Collections.Generic.Dictionary<string, int> inventory)
        {
            if (recipe == null) return false;
            
            // Check if recipe is available at this station
            if (!string.IsNullOrEmpty(recipe.RequiredStationType) && 
                recipe.RequiredStationType != stationType)
            {
                return false;
            }
            
            // Check if player has ingredients
            if (!recipe.CanCraft(inventory))
            {
                return false;
            }
            
            // Craft the item (would consume ingredients and create result)
            if (craftingManager != null)
            {
                return craftingManager.ProcessCrafting(recipe, this);
            }
            
            return false;
        }
    }
}
