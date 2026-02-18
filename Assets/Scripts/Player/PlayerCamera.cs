using UnityEngine;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Player
{
    /// <summary>
    /// Camera controller with first-person and third-person toggle functionality.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Player configuration asset")]
        [SerializeField] private PlayerConfig config;
        
        [Header("Player Reference")]
        [Tooltip("Player transform to follow")]
        [SerializeField] private Transform playerTransform;
        
        [Header("First Person")]
        [Tooltip("Transform representing player head (for first-person camera position)")]
        [SerializeField] private Transform playerHead;
        
        [Header("Third Person")]
        [Tooltip("Camera pivot point for third-person (should be child of player)")]
        [SerializeField] private Transform cameraPivot;
        
        private Camera cam;
        private bool isFirstPerson = true;
        private float currentVerticalAngle = 0f;
        private float currentHorizontalAngle = 0f;
        
        /// <summary>
        /// Whether the camera is in first-person mode.
        /// </summary>
        public bool IsFirstPerson => isFirstPerson;
        
        private void Awake()
        {
            cam = GetComponent<Camera>();
            
            if (playerTransform == null)
            {
                playerTransform = transform.parent;
            }
            
            // Initialize camera position
            if (isFirstPerson && playerHead != null)
            {
                transform.position = playerHead.position;
                transform.SetParent(playerHead);
            }
        }
        
        private void Start()
        {
            if (config != null)
            {
                cam.fieldOfView = isFirstPerson ? config.FirstPersonFOV : config.ThirdPersonFOV;
            }
            
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void Update()
        {
            HandleCameraToggle();
            HandleCameraRotation();
            HandleCameraPosition();
        }
        
        /// <summary>
        /// Handles toggling between first-person and third-person modes.
        /// </summary>
        private void HandleCameraToggle()
        {
            if (config == null) return;
            
            if (Input.GetKeyDown(config.CameraToggleKey))
            {
                ToggleCameraMode();
            }
        }
        
        /// <summary>
        /// Toggles between first-person and third-person camera modes.
        /// </summary>
        public void ToggleCameraMode()
        {
            isFirstPerson = !isFirstPerson;
            
            if (config != null)
            {
                cam.fieldOfView = isFirstPerson ? config.FirstPersonFOV : config.ThirdPersonFOV;
            }
            
            if (isFirstPerson)
            {
                // Switch to first-person
                if (playerHead != null)
                {
                    transform.SetParent(playerHead);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                // Switch to third-person
                if (cameraPivot != null)
                {
                    transform.SetParent(cameraPivot);
                    transform.localPosition = Vector3.zero;
                }
                else if (playerTransform != null)
                {
                    transform.SetParent(playerTransform);
                    // Position camera behind player
                    if (config != null)
                    {
                        transform.localPosition = new Vector3(0f, config.ThirdPersonHeightOffset, -config.ThirdPersonDistance);
                    }
                }
            }
        }
        
        /// <summary>
        /// Handles camera rotation based on mouse input.
        /// </summary>
        private void HandleCameraRotation()
        {
            if (config == null) return;
            
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            
            float sensitivity = isFirstPerson ? config.FirstPersonSensitivity : config.ThirdPersonSensitivity;
            
            // Rotate player horizontally (Y-axis)
            if (playerTransform != null)
            {
                playerTransform.Rotate(Vector3.up * mouseX * sensitivity);
            }
            
            // Rotate camera vertically (X-axis)
            if (isFirstPerson)
            {
                // First-person: rotate camera directly
                currentVerticalAngle -= mouseY * sensitivity;
                currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -90f, 90f);
                transform.localRotation = Quaternion.Euler(currentVerticalAngle, 0f, 0f);
            }
            else
            {
                // Third-person: rotate around pivot
                currentVerticalAngle -= mouseY * sensitivity;
                currentVerticalAngle = Mathf.Clamp(
                    currentVerticalAngle,
                    config.ThirdPersonMinAngle,
                    config.ThirdPersonMaxAngle
                );
                
                if (cameraPivot != null)
                {
                    cameraPivot.localRotation = Quaternion.Euler(currentVerticalAngle, 0f, 0f);
                }
            }
        }
        
        /// <summary>
        /// Handles camera position updates (for third-person follow).
        /// </summary>
        private void HandleCameraPosition()
        {
            if (isFirstPerson || config == null || playerTransform == null) return;
            
            // Third-person camera positioning
            if (cameraPivot == null)
            {
                // If no pivot, position relative to player
                Vector3 targetPosition = playerTransform.position + 
                    Vector3.up * config.ThirdPersonHeightOffset + 
                    -transform.forward * config.ThirdPersonDistance;
                
                // Raycast to prevent camera clipping through walls
                RaycastHit hit;
                if (Physics.Raycast(playerTransform.position + Vector3.up * config.ThirdPersonHeightOffset, 
                    -transform.forward, out hit, config.ThirdPersonDistance))
                {
                    targetPosition = hit.point + transform.forward * 0.5f;
                }
                
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            }
        }
    }
}
