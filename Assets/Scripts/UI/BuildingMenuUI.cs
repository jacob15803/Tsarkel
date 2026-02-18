using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Systems.Building;
using Tsarkel.ScriptableObjects.Buildings;

namespace Tsarkel.UI
{
    /// <summary>
    /// UI component for building selection menu.
    /// </summary>
    public class BuildingMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Building button prefab")]
        [SerializeField] private GameObject buildingButtonPrefab;
        
        [Tooltip("Container for building buttons")]
        [SerializeField] private Transform buttonContainer;
        
        [Tooltip("Close button")]
        [SerializeField] private Button closeButton;
        
        [Header("Building Data")]
        [Tooltip("List of available buildings")]
        [SerializeField] private BuildingData[] availableBuildings;
        
        private BuildingManager buildingManager;
        
        private void Start()
        {
            buildingManager = FindObjectOfType<BuildingManager>();
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseMenu);
            }
            
            CreateBuildingButtons();
        }
        
        /// <summary>
        /// Creates buttons for each available building.
        /// </summary>
        private void CreateBuildingButtons()
        {
            if (buildingButtonPrefab == null || buttonContainer == null) return;
            
            foreach (BuildingData buildingData in availableBuildings)
            {
                if (buildingData == null) continue;
                
                GameObject buttonObj = Instantiate(buildingButtonPrefab, buttonContainer);
                Button button = buttonObj.GetComponent<Button>();
                
                if (button != null)
                {
                    button.onClick.AddListener(() => SelectBuilding(buildingData));
                }
                
                // Set button text/image if available
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = buildingData.BuildingName;
                }
                
                Image buttonImage = buttonObj.GetComponent<Image>();
                if (buttonImage != null && buildingData.Icon != null)
                {
                    buttonImage.sprite = buildingData.Icon;
                }
            }
        }
        
        /// <summary>
        /// Selects a building and enters placement mode.
        /// </summary>
        private void SelectBuilding(BuildingData buildingData)
        {
            if (buildingManager != null)
            {
                buildingManager.EnterPlacementMode(buildingData);
            }
            
            CloseMenu();
        }
        
        /// <summary>
        /// Closes the building menu.
        /// </summary>
        private void CloseMenu()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Opens the building menu.
        /// </summary>
        public void OpenMenu()
        {
            gameObject.SetActive(true);
        }
    }
}
