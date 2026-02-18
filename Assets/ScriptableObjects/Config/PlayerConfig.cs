using UnityEngine;

namespace Tsarkel.ScriptableObjects.Config
{
    /// <summary>
    /// Configuration data for player movement and camera settings.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Tsarkel/Config/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Base movement speed (units per second)")]
        [SerializeField] private float walkSpeed = 5f;
        
        [Tooltip("Sprint speed multiplier")]
        [SerializeField] private float sprintMultiplier = 1.5f;
        
        [Tooltip("Jump height (units)")]
        [SerializeField] private float jumpHeight = 2f;
        
        [Tooltip("Gravity multiplier")]
        [SerializeField] private float gravityMultiplier = 2f;
        
        [Tooltip("Ground check distance")]
        [SerializeField] private float groundCheckDistance = 0.1f;
        
        [Header("Camera - First Person")]
        [Tooltip("First person camera field of view")]
        [SerializeField] private float firstPersonFOV = 75f;
        
        [Tooltip("First person mouse sensitivity")]
        [SerializeField] private float firstPersonSensitivity = 2f;
        
        [Tooltip("First person camera height offset from player center")]
        [SerializeField] private float firstPersonHeightOffset = 1.6f;
        
        [Header("Camera - Third Person")]
        [Tooltip("Third person camera field of view")]
        [SerializeField] private float thirdPersonFOV = 60f;
        
        [Tooltip("Third person mouse sensitivity")]
        [SerializeField] private float thirdPersonSensitivity = 2f;
        
        [Tooltip("Distance from player in third person")]
        [SerializeField] private float thirdPersonDistance = 5f;
        
        [Tooltip("Height offset for third person camera")]
        [SerializeField] private float thirdPersonHeightOffset = 2f;
        
        [Tooltip("Minimum vertical angle for third person camera")]
        [SerializeField] private float thirdPersonMinAngle = -30f;
        
        [Tooltip("Maximum vertical angle for third person camera")]
        [SerializeField] private float thirdPersonMaxAngle = 60f;
        
        [Header("Input")]
        [Tooltip("Key code for toggling camera perspective")]
        [SerializeField] private KeyCode cameraToggleKey = KeyCode.V;
        
        [Tooltip("Key code for sprinting")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        
        [Tooltip("Key code for jumping")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;

        // Public properties
        public float WalkSpeed => walkSpeed;
        public float SprintMultiplier => sprintMultiplier;
        public float JumpHeight => jumpHeight;
        public float GravityMultiplier => gravityMultiplier;
        public float GroundCheckDistance => groundCheckDistance;
        public float FirstPersonFOV => firstPersonFOV;
        public float FirstPersonSensitivity => firstPersonSensitivity;
        public float FirstPersonHeightOffset => firstPersonHeightOffset;
        public float ThirdPersonFOV => thirdPersonFOV;
        public float ThirdPersonSensitivity => thirdPersonSensitivity;
        public float ThirdPersonDistance => thirdPersonDistance;
        public float ThirdPersonHeightOffset => thirdPersonHeightOffset;
        public float ThirdPersonMinAngle => thirdPersonMinAngle;
        public float ThirdPersonMaxAngle => thirdPersonMaxAngle;
        public KeyCode CameraToggleKey => cameraToggleKey;
        public KeyCode SprintKey => sprintKey;
        public KeyCode JumpKey => jumpKey;
        
        /// <summary>
        /// Gets the sprint speed based on walk speed and sprint multiplier.
        /// </summary>
        public float GetSprintSpeed()
        {
            return walkSpeed * sprintMultiplier;
        }
    }
}
