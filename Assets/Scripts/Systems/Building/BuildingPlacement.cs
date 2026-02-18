using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Buildings;
using Tsarkel.Systems.Tsunami;

namespace Tsarkel.Systems.Building
{
    /// <summary>
    /// Handles building placement with ghost preview, snap-to-ground, and validation.
    /// </summary>
    public class BuildingPlacement : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Safe elevation detector reference")]
        [SerializeField] private SafeElevationDetector elevationDetector;
        
        [Header("Placement Settings")]
        [Tooltip("Layer mask for ground detection")]
        [SerializeField] private LayerMask groundLayerMask = 1;
        
        [Tooltip("Maximum raycast distance for ground detection")]
        [SerializeField] private float maxRaycastDistance = 100f;
        
        [Tooltip("Material for ghost preview (semi-transparent)")]
        [SerializeField] private Material ghostMaterial;
        
        private BuildingData currentBuildingData;
        private GameObject ghostPreview;
        private bool isInPlacementMode = false;
        private Camera playerCamera;
        
        /// <summary>
        /// Whether placement mode is active.
        /// </summary>
        public bool IsInPlacementMode => isInPlacementMode;
        
        private void Start()
        {
            playerCamera = Camera.main;
            
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            if (elevationDetector == null)
            {
                elevationDetector = FindObjectOfType<SafeElevationDetector>();
            }
        }
        
        private void Update()
        {
            if (isInPlacementMode && currentBuildingData != null)
            {
                UpdateGhostPreview();
                HandlePlacementInput();
            }
        }
        
        /// <summary>
        /// Sets the building data for placement.
        /// </summary>
        public void SetBuildingData(BuildingData buildingData)
        {
            currentBuildingData = buildingData;
            
            // Create ghost preview if needed
            if (ghostPreview != null)
            {
                Destroy(ghostPreview);
            }
            
            if (buildingData != null && buildingData.Prefab != null)
            {
                ghostPreview = Instantiate(buildingData.Prefab);
                
                // Make it a ghost (semi-transparent, no collisions)
                SetGhostProperties(ghostPreview);
            }
        }
        
        /// <summary>
        /// Sets placement mode on/off.
        /// </summary>
        public void SetPlacementMode(bool active)
        {
            isInPlacementMode = active;
            
            if (!active && ghostPreview != null)
            {
                Destroy(ghostPreview);
                ghostPreview = null;
            }
        }
        
        /// <summary>
        /// Updates the ghost preview position based on mouse/raycast.
        /// </summary>
        private void UpdateGhostPreview()
        {
            if (ghostPreview == null || playerCamera == null) return;
            
            // Raycast from camera through mouse position
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, groundLayerMask))
            {
                Vector3 placementPosition = hit.point;
                
                // Snap to grid if configured
                if (currentBuildingData.GridSnapSize > 0f)
                {
                    placementPosition = SnapToGrid(placementPosition, currentBuildingData.GridSnapSize);
                }
                
                // Snap to ground
                placementPosition = SnapToGround(placementPosition);
                
                // Validate placement
                bool canPlace = ValidatePlacement(placementPosition);
                
                // Update ghost position and color
                ghostPreview.transform.position = placementPosition;
                UpdateGhostColor(canPlace);
            }
        }
        
        /// <summary>
        /// Snaps position to grid.
        /// </summary>
        private Vector3 SnapToGrid(Vector3 position, float gridSize)
        {
            position.x = Mathf.Round(position.x / gridSize) * gridSize;
            position.z = Mathf.Round(position.z / gridSize) * gridSize;
            return position;
        }
        
        /// <summary>
        /// Snaps position to ground using raycast.
        /// </summary>
        private Vector3 SnapToGround(Vector3 position)
        {
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(position.x, position.y + 10f, position.z);
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, maxRaycastDistance, groundLayerMask))
            {
                return hit.point;
            }
            
            return position;
        }
        
        /// <summary>
        /// Validates if a building can be placed at the given position.
        /// </summary>
        private bool ValidatePlacement(Vector3 position)
        {
            if (currentBuildingData == null || elevationDetector == null) return false;
            
            // Check elevation
            if (!elevationDetector.CanPlaceBuildingAt(position, currentBuildingData.MinElevation))
            {
                return false;
            }
            
            // Check if position is on water (if not allowed)
            if (!currentBuildingData.CanPlaceOnWater && position.y <= elevationDetector.CurrentWaterLevel)
            {
                return false;
            }
            
            // Check ground angle if required
            if (currentBuildingData.RequiresFlatGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, 1f, groundLayerMask))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    if (angle > currentBuildingData.MaxGroundAngle)
                    {
                        return false;
                    }
                }
            }
            
            // Check for collisions (optional - can be expanded)
            // For now, we'll assume placement is valid if above checks pass
            
            return true;
        }
        
        /// <summary>
        /// Updates ghost preview color based on placement validity.
        /// </summary>
        private void UpdateGhostColor(bool canPlace)
        {
            if (ghostPreview == null) return;
            
            Renderer[] renderers = ghostPreview.GetComponentsInChildren<Renderer>();
            Color color = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            
            foreach (Renderer renderer in renderers)
            {
                if (ghostMaterial != null)
                {
                    renderer.material = ghostMaterial;
                }
                
                foreach (Material mat in renderer.materials)
                {
                    mat.color = color;
                }
            }
        }
        
        /// <summary>
        /// Sets ghost properties (no collisions, semi-transparent).
        /// </summary>
        private void SetGhostProperties(GameObject obj)
        {
            // Disable colliders
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            
            // Make semi-transparent
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (ghostMaterial != null)
                {
                    renderer.material = ghostMaterial;
                }
                
                foreach (Material mat in renderer.materials)
                {
                    Color color = mat.color;
                    color.a = 0.5f;
                    mat.color = color;
                    
                    // Enable transparency
                    if (mat.HasProperty("_Mode"))
                    {
                        mat.SetFloat("_Mode", 3); // Transparent mode
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles placement input (left click to place).
        /// </summary>
        private void HandlePlacementInput()
        {
            if (Input.GetMouseButtonDown(0)) // Left click
            {
                if (ghostPreview != null)
                {
                    Vector3 placementPosition = ghostPreview.transform.position;
                    
                    if (ValidatePlacement(placementPosition))
                    {
                        PlaceBuilding(placementPosition);
                    }
                }
            }
        }
        
        /// <summary>
        /// Places the building at the specified position.
        /// </summary>
        private void PlaceBuilding(Vector3 position)
        {
            if (currentBuildingData == null || currentBuildingData.Prefab == null) return;
            
            // Instantiate building
            GameObject building = Instantiate(currentBuildingData.Prefab, position, Quaternion.identity);
            
            // Apply building tag
            if (!string.IsNullOrEmpty(currentBuildingData.BuildingTag))
            {
                building.tag = currentBuildingData.BuildingTag;
            }
            
            // Add BuildingDurability component if not present
            BuildingDurability durability = building.GetComponent<BuildingDurability>();
            if (durability == null)
            {
                durability = building.AddComponent<BuildingDurability>();
            }
            
            durability.Initialize(currentBuildingData);
            
            // Invoke building placed event
            EventManager.Instance.InvokeBuildingPlaced(building, currentBuildingData);
            
            // Note: Resource consumption would be handled here in a full implementation
        }
    }
}
