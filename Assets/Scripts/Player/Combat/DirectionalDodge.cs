using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.ScriptableObjects.Combat;
using Tsarkel.Systems.Combat;

namespace Tsarkel.Player.Combat
{
    /// <summary>
    /// Attached to the Player GameObject.
    /// Listens for incoming attack telegraphs and captures WASD input during
    /// the reaction window to determine the dodge outcome.
    ///
    /// Direction mapping (camera-relative):
    ///   W → Forward dodge
    ///   S → Backward dodge
    ///   A → Left dodge
    ///   D → Right dodge
    ///   Space → Jump dodge (universal, costs stamina)
    /// </summary>
    public class DirectionalDodge : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Configuration")]
        [Tooltip("Combat configuration asset")]
        [SerializeField] private CombatConfig config;

        [Header("Dependencies")]
        [Tooltip("Camera transform used for direction calculations")]
        [SerializeField] private Transform cameraTransform;

        [Tooltip("Stamina system for jump dodge cost")]
        [SerializeField] private Systems.Survival.StaminaSystem staminaSystem;

        // ─── State ────────────────────────────────────────────────────────────
        private bool windowOpen = false;
        private float windowTimer = 0f;
        private AttackDirection pendingAttackDirection;
        private AttackDirection? playerInputDirection = null;
        private bool jumpDodgePressed = false;

        // Outcome stored until EnemyAttackTelegraph queries it
        private DodgeOutcome pendingOutcome = DodgeOutcome.Failed;
        private bool outcomeReady = false;

        // Invincibility frames after a perfect dodge
        private bool isInvincible = false;
        private float invincibilityTimer = 0f;

        /// <summary>Whether the player currently has invincibility frames active.</summary>
        public bool IsInvincible => isInvincible;

        /// <summary>Whether a reaction window is currently open.</summary>
        public bool IsWindowOpen => windowOpen;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            if (cameraTransform == null)
            {
                Camera cam = Camera.main;
                if (cam != null) cameraTransform = cam.transform;
            }

            if (staminaSystem == null)
                staminaSystem = GetComponent<Systems.Survival.StaminaSystem>();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnAttackTelegraphed += HandleAttackTelegraphed;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnAttackTelegraphed -= HandleAttackTelegraphed;
        }

        private void Update()
        {
            HandleInvincibilityTimer();

            if (!windowOpen) return;

            CaptureDirectionalInput();
            TickWindowTimer();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Called by EnemyAttackTelegraph after the reaction window closes.
        /// Returns the dodge outcome and resets state.
        /// </summary>
        public DodgeOutcome ConsumeOutcome(AttackDirection attackDirection)
        {
            if (!outcomeReady)
            {
                // Window may have closed without input — evaluate now
                pendingOutcome = EvaluateOutcome(attackDirection, playerInputDirection, jumpDodgePressed);
            }

            DodgeOutcome result = pendingOutcome;
            ResetState();

            // Apply invincibility on perfect dodge
            if (result == DodgeOutcome.Perfect && config != null)
            {
                isInvincible = true;
                invincibilityTimer = config.PerfectDodgeInvincibilityDuration;
            }

            // Fire event
            AttackDirection inputDir = playerInputDirection ?? attackDirection; // fallback
            EventManager.Instance.InvokeDodgeAttempted(inputDir, attackDirection, result);

            return result;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        /// <summary>
        /// Opens the reaction window when an attack is telegraphed.
        /// </summary>
        private void HandleAttackTelegraphed(AttackDirection direction, float telegraphDuration)
        {
            // Only open window if in combat mode
            if (!GetComponent<PlayerController>()?.IsInCombatMode ?? false) return;

            pendingAttackDirection = direction;
            playerInputDirection = null;
            jumpDodgePressed = false;
            outcomeReady = false;

            float windowDuration = config != null ? config.ReactionWindowDuration : 0.5f;
            windowTimer = telegraphDuration + windowDuration; // Opens at end of telegraph
            windowOpen = true;
        }

        // ─── Input Capture ────────────────────────────────────────────────────

        /// <summary>
        /// Reads WASD and Space input and records the last pressed direction.
        /// Only the most recent input is kept (last valid input wins).
        /// </summary>
        private void CaptureDirectionalInput()
        {
            // Jump dodge — universal, costs stamina
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool hasStamina = staminaSystem == null ||
                    staminaSystem.CurrentStamina >= (config != null ? config.JumpDodgeStaminaCost : 15f);

                if (hasStamina)
                {
                    jumpDodgePressed = true;
                    if (staminaSystem != null && config != null)
                        staminaSystem.SetStamina(staminaSystem.CurrentStamina - config.JumpDodgeStaminaCost);
                }
            }

            // Directional dodge — camera-relative
            if (Input.GetKeyDown(KeyCode.W)) playerInputDirection = AttackDirection.Front;
            else if (Input.GetKeyDown(KeyCode.S)) playerInputDirection = AttackDirection.Back;
            else if (Input.GetKeyDown(KeyCode.A)) playerInputDirection = AttackDirection.Left;
            else if (Input.GetKeyDown(KeyCode.D)) playerInputDirection = AttackDirection.Right;
        }

        // ─── Window Timer ─────────────────────────────────────────────────────

        private void TickWindowTimer()
        {
            windowTimer -= Time.unscaledDeltaTime;
            if (windowTimer <= 0f)
            {
                // Window expired — evaluate outcome with whatever input was captured
                pendingOutcome = EvaluateOutcome(pendingAttackDirection, playerInputDirection, jumpDodgePressed);
                outcomeReady = true;
                windowOpen = false;
            }
        }

        // ─── Outcome Evaluation ───────────────────────────────────────────────

        /// <summary>
        /// Determines the dodge outcome based on player input vs. attack direction.
        ///
        /// Logic:
        ///   Jump dodge → Perfect (universal)
        ///   Correct direction → Perfect
        ///   Perpendicular direction (side dodge) → Partial
        ///   Opposite direction → Failed
        ///   No input → Failed
        /// </summary>
        private DodgeOutcome EvaluateOutcome(AttackDirection attackDir,
                                              AttackDirection? inputDir,
                                              bool jumpDodge)
        {
            // Jump dodge is always perfect
            if (jumpDodge) return DodgeOutcome.Perfect;

            if (inputDir == null) return DodgeOutcome.Failed;

            AttackDirection input = inputDir.Value;

            // Correct direction: dodge away from the attack
            // e.g., attack from Front → dodge Backward (S = Back)
            AttackDirection correctDodge = GetCorrectDodgeDirection(attackDir);

            if (input == correctDodge) return DodgeOutcome.Perfect;

            // Side dodge (perpendicular) → Partial
            if (IsPerpendicularDodge(attackDir, input)) return DodgeOutcome.Partial;

            // Wrong or opposite direction → Failed
            return DodgeOutcome.Failed;
        }

        /// <summary>
        /// Returns the correct dodge direction for a given attack direction.
        /// Player should dodge away from the attack:
        ///   Front attack → dodge Back (S)
        ///   Back attack → dodge Front (W)
        ///   Left attack → dodge Right (D)
        ///   Right attack → dodge Left (A)
        /// </summary>
        private AttackDirection GetCorrectDodgeDirection(AttackDirection attackDir)
        {
            switch (attackDir)
            {
                case AttackDirection.Front: return AttackDirection.Back;
                case AttackDirection.Back:  return AttackDirection.Front;
                case AttackDirection.Left:  return AttackDirection.Right;
                case AttackDirection.Right: return AttackDirection.Left;
                default: return AttackDirection.Back;
            }
        }

        /// <summary>
        /// Returns true if the input direction is perpendicular to the attack direction.
        /// </summary>
        private bool IsPerpendicularDodge(AttackDirection attackDir, AttackDirection inputDir)
        {
            bool attackIsFrontBack = attackDir == AttackDirection.Front || attackDir == AttackDirection.Back;
            bool inputIsLeftRight  = inputDir  == AttackDirection.Left  || inputDir  == AttackDirection.Right;
            bool attackIsLeftRight = attackDir == AttackDirection.Left  || attackDir == AttackDirection.Right;
            bool inputIsFrontBack  = inputDir  == AttackDirection.Front || inputDir  == AttackDirection.Back;

            return (attackIsFrontBack && inputIsLeftRight) || (attackIsLeftRight && inputIsFrontBack);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void HandleInvincibilityTimer()
        {
            if (!isInvincible) return;
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
                isInvincible = false;
        }

        private void ResetState()
        {
            windowOpen = false;
            windowTimer = 0f;
            playerInputDirection = null;
            jumpDodgePressed = false;
            outcomeReady = false;
        }
    }
}
