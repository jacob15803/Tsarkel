using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Tsarkel.ScriptableObjects.Crafting;

namespace Tsarkel.Systems.Crafting
{
    /// <summary>
    /// UI component for crafting interface.
    /// Displays available recipes and handles crafting input.
    /// </summary>
    public class CraftingUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Recipe button prefab")]
        [SerializeField] private GameObject recipeButtonPrefab;
        
        [Tooltip("Recipe list container")]
        [SerializeField] private Transform recipeListContainer;
        
        [Tooltip("Close button")]
        [SerializeField] private Button closeButton;
        
        [Header("Dependencies")]
        [Tooltip("Crafting manager reference")]
        [SerializeField] private CraftingManager craftingManager;
        
        [Tooltip("Current crafting station")]
        [SerializeField] private CraftingStation currentStation;
        
        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseUI);
            }
        }
        
        /// <summary>
        /// Opens the crafting UI for a specific station.
        /// </summary>
        public void OpenUI(CraftingStation station)
        {
            currentStation = station;
            gameObject.SetActive(true);
            
            if (station != null)
            {
                DisplayRecipes(station.AvailableRecipes);
            }
        }
        
        /// <summary>
        /// Closes the crafting UI.
        /// </summary>
        public void CloseUI()
        {
            gameObject.SetActive(false);
            currentStation = null;
        }
        
        /// <summary>
        /// Displays available recipes.
        /// </summary>
        private void DisplayRecipes(CraftingRecipe[] recipes)
        {
            if (recipeListContainer == null || recipeButtonPrefab == null) return;
            
            // Clear existing buttons
            foreach (Transform child in recipeListContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create buttons for each recipe
            if (recipes != null)
            {
                foreach (var recipe in recipes)
                {
                    if (recipe == null) continue;
                    
                    GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
                    Button button = buttonObj.GetComponent<Button>();
                    
                    if (button != null)
                    {
                        button.onClick.AddListener(() => OnRecipeSelected(recipe));
                    }
                    
                    // Set button text
                    Text buttonText = buttonObj.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = recipe.RecipeName;
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles recipe selection.
        /// </summary>
        private void OnRecipeSelected(CraftingRecipe recipe)
        {
            if (currentStation == null || recipe == null) return;
            
            // Get player inventory (would need inventory system)
            Dictionary<string, int> inventory = new Dictionary<string, int>();
            
            // Attempt to craft
            if (currentStation.CraftItem(recipe, inventory))
            {
                // Success - update UI
                Debug.Log($"Crafted: {recipe.ResultItem.ItemName}");
            }
            else
            {
                // Failed - show error
                Debug.Log("Cannot craft: Missing ingredients or invalid station");
            }
        }
    }
}
