using UnityEngine;
using Tsarkel.ScriptableObjects.Config;
using Tsarkel.Systems.Survival;
using Tsarkel.Systems.Tsunami;

namespace Tsarkel.Player
{
    /// <summary>
    /// Main player movement controller using CharacterController.
    /// Handles movement, sprinting, jumping, and tsunami force application.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Player configuration asset")]
        [SerializeField] private PlayerConfig config;
        
        [Header("Dependencies")]
        [Tooltip("Stamina system reference")]
        [SerializeField] private StaminaSystem staminaSystem;
        
        [Tooltip("Safe elevation detector reference")]
        [SerializeField] private SafeElevationDetector elevationDetector;
        
        [Header("Ground Check")]
        [Tooltip("Layer mask for ground detection")]
        [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
        
        private CharacterController characterController;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isSprinting;
        private Vector3 externalForce; // For tsunami impacts
        private bool isInCombatMode;   // Suppresses normal WASD movement during combat
        
        /// <summary>
        /// Whether the player is currently grounded.
        /// </summary>
        public bool IsGrounded => isGrounded;
        
        /// <summary>
        /// Whether the player is currently sprinting.
        /// </summary>
        public bool IsSprinting => isSprinting;
        
        /// <summary>
        /// Current movement velocity.
        /// </summary>
        public Vector3 Velocity => velocity;
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            if (staminaSystem == null)
            {
                staminaSystem = GetComponent<StaminaSystem>();
            }
            
            if (elevationDetector == null)
            {
                elevationDetector = FindObjectOfType<SafeElevationDetector>();
            }
        }
        
        private void Update()
        {
            HandleGroundCheck();
            HandleMovement();
            HandleGravity();
            HandleExternalForces();
            
            // Apply movement
            characterController.Move(velocity * Time.deltaTime);
        }
        
        /// <summary>
        /// Checks if the player is on the ground.
        /// </summary>
        private void HandleGroundCheck()
        {
            float checkDistance = config != null ? config.GroundCheckDistance : 0.1f;
            isGrounded = characterController.isGrounded || 
                        Physics.CheckSphere(
                            transform.position + Vector3.down * (characterController.height * 0.5f + checkDistance),
                            checkDistance,
                            groundLayerMask
                        );
        }
        
        /// <summary>
        /// Enables or disables combat mode.
        /// While in combat mode, normal WASD movement is suppressed so the
        /// DirectionalDodge component can own those inputs.
        /// </summary>
        public void SetCombatMode(bool active)
        {
            isInCombatMode = active;
            if (!active)
            {
                // Clear horizontal velocity when exiting combat so player doesn't slide
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }

        /// <summary>Whether the player is currently in combat mode.</summary>
        public bool IsInCombatMode => isInCombatMode;

        /// <summary>
        /// Handles player movement input and sprinting.
        /// </summary>
        private void HandleMovement()
        {
            if (config == null) return;

            // Combat mode suppresses normal movement — DirectionalDodge handles input instead
            if (isInCombatMode)
            {
                velocity.x = 0f;
                velocity.z = 0f;
                return;
            }

            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            // Calculate movement direction relative to camera
            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
            moveDirection.Normalize();
            
            // Check sprint input
            bool sprintInput = Input.GetKey(config.SprintKey);
            bool canSprint = staminaSystem != null && staminaSystem.CanSprint;
            
            // Determine if sprinting
            isSprinting = sprintInput && canSprint && moveDirection.magnitude > 0.1f;
            
            // Update stamina system
            if (staminaSystem != null)
            {
                if (isSprinting)
                {
                    staminaSystem.StartSprinting();
                    staminaSystem.UpdateSprinting();
                }
                else
                {
                    staminaSystem.StopSprinting();
                }
            }
            
            // Calculate movement speed
            float speed = isSprinting ? config.GetSprintSpeed() : config.WalkSpeed;
            
            // Apply movement to velocity (preserve Y for gravity)
            velocity.x = moveDirection.x * speed;
            velocity.z = moveDirection.z * speed;
        }
        
        /// <summary>
        /// Handles gravity and jumping.
        /// </summary>
        private void HandleGravity()
        {
            if (config == null) return;
            
            float gravity = Physics.gravity.y * config.GravityMultiplier;
            
            if (isGrounded)
            {
                // Reset Y velocity when grounded (unless jumping)
                if (velocity.y < 0f)
                {
                    velocity.y = -2f; // Small downward force to keep grounded
                }
                
                // Handle jump input
                if (Input.GetKeyDown(config.JumpKey))
                {
                    velocity.y = Mathf.Sqrt(config.JumpHeight * -2f * gravity);
                }
            }
            else
            {
                // Apply gravity when in air
                velocity.y += gravity * Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Handles external forces (e.g., from tsunami).
        /// </summary>
        private void HandleExternalForces()
        {
            if (externalForce.magnitude > 0.01f)
            {
                velocity += externalForce * Time.deltaTime;
                
                // Decay external force
                externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 2f);
            }
        }
        
        /// <summary>
        /// Applies an external force to the player (e.g., from tsunami impact).
        /// </summary>
        /// <param name="force">Force vector to apply</param>
        public void ApplyForce(Vector3 force)
        {
            externalForce += force;
        }
        
        /// <summary>
        /// Gets the player's current elevation.
        /// </summary>
        public float GetElevation()
        {
            return transform.position.y;
        }
        
        /// <summary>
        /// Checks if the player is at a safe elevation.
        /// </summary>
        public bool IsAtSafeElevation()
        {
            if (elevationDetector == null) return true;
            return elevationDetector.IsElevationSafe(transform.position);
        }
    }
}
