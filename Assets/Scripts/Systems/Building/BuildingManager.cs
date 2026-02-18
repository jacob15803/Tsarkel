using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Buildings;

namespace Tsarkel.Systems.Building
{
    /// <summary>
    /// Manages building placement mode and coordinates building system.
    /// </summary>
    public class BuildingManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Building placement system reference")]
        [SerializeField] private BuildingPlacement buildingPlacement;
        
        private BuildingData currentBuildingData;
        private bool isInPlacementMode = false;
        
        /// <summary>
        /// Whether the player is currently in building placement mode.
        /// </summary>
        public bool IsInPlacementMode => isInPlacementMode;
        
        /// <summary>
        /// Currently selected building data.
        /// </summary>
        public BuildingData CurrentBuildingData => currentBuildingData;
        
        private void Awake()
        {
            if (buildingPlacement == null)
            {
                buildingPlacement = FindObjectOfType<BuildingPlacement>();
            }
        }
        
        /// <summary>
        /// Enters building placement mode with the specified building.
        /// </summary>
        /// <param name="buildingData">Building data to place</param>
        public void EnterPlacementMode(BuildingData buildingData)
        {
            if (buildingData == null)
            {
                Debug.LogWarning("BuildingManager: Cannot enter placement mode with null building data.");
                return;
            }
            
            currentBuildingData = buildingData;
            isInPlacementMode = true;
            
            if (buildingPlacement != null)
            {
                buildingPlacement.SetBuildingData(buildingData);
                buildingPlacement.SetPlacementMode(true);
            }
            
            EventManager.Instance.InvokeBuildingPlacementModeChanged(true, buildingData);
        }
        
        /// <summary>
        /// Exits building placement mode.
        /// </summary>
        public void ExitPlacementMode()
        {
            isInPlacementMode = false;
            currentBuildingData = null;
            
            if (buildingPlacement != null)
            {
                buildingPlacement.SetPlacementMode(false);
            }
            
            EventManager.Instance.InvokeBuildingPlacementModeChanged(false, null);
        }
        
        /// <summary>
        /// Toggles building placement mode.
        /// </summary>
        public void TogglePlacementMode(BuildingData buildingData)
        {
            if (isInPlacementMode)
            {
                ExitPlacementMode();
            }
            else
            {
                EnterPlacementMode(buildingData);
            }
        }
        
        private void Update()
        {
            // Exit placement mode on Escape key
            if (isInPlacementMode && Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPlacementMode();
            }
        }
    }
}
