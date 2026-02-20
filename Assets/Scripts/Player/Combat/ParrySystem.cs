using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.ScriptableObjects.Combat;
using Tsarkel.Systems.Combat;
using Tsarkel.Systems.Survival;

namespace Tsarkel.Player.Combat
{
    /// <summary>
    /// Attached to the Player GameObject.
    /// Handles directional parry input: Right Mouse Button + correct WASD direction
    /// pressed within the parry timing window.
    ///
    /// Parry success → enemy stunned, no damage.
    /// Parry failure → heavy damage penalty, player staggered.
    /// </summary>
    public class ParrySystem : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Configuration")]
        [Tooltip("Combat configuration asset")]
        [SerializeField] private CombatConfig config;

        [Header("Dependencies")]
        [Tooltip("Stamina system for parry cost")]
        [SerializeField] private StaminaSystem staminaSystem;

        [Tooltip("Player stats for stagger application")]
        [SerializeField] private PlayerStats playerStats;

        // ─── State ────────────────────────────────────────────────────────────
        private ParryWindow activeWindow = null;
        private bool isStaggered = false;
        private float staggerTimer = 0f;

        /// <summary>Whether a parry window is currently open.</summary>
        public bool HasActiveWindow => activeWindow != null && activeWindow.IsOpen;

        /// <summary>Whether the player is currently staggered from a failed parry.</summary>
        public bool IsStaggered => isStaggered;

        /// <summary>
        /// Normalised progress through the current parry window (0–1).
        /// 0 = just opened, 1 = closed. Returns 1 if no window is open.
        /// </summary>
        public float WindowProgress => activeWindow != null ? activeWindow.Progress : 1f;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            if (staminaSystem == null) staminaSystem = GetComponent<StaminaSystem>();
            if (playerStats == null)   playerStats   = GetComponent<PlayerStats>();
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
            TickStagger();

            if (activeWindow == null) return;

            activeWindow.Tick(Time.unscaledDeltaTime);

            // Check parry input while window is open and not already attempted
            if (activeWindow.IsOpen && !activeWindow.WasAttempted)
                CheckParryInput();

            // Window expired without attempt — clean up
            if (!activeWindow.IsOpen)
                activeWindow = null;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleAttackTelegraphed(AttackDirection direction, float telegraphDuration)
        {
            // Only open a parry window if in combat mode
            if (!GetComponent<PlayerController>()?.IsInCombatMode ?? false) return;

            float windowDuration = config != null ? config.ParryWindowDuration : 0.45f;
            // Parry window opens at the same time as the reaction window (after telegraph)
            // We delay opening by telegraph duration using a coroutine
            StartCoroutine(OpenWindowAfterDelay(direction, telegraphDuration, windowDuration));
        }

        private System.Collections.IEnumerator OpenWindowAfterDelay(
            AttackDirection direction, float delay, float windowDuration)
        {
            yield return new WaitForSecondsRealtime(delay);
            activeWindow = new ParryWindow(direction, windowDuration);
        }

        // ─── Input Handling ───────────────────────────────────────────────────

        /// <summary>
        /// Checks for Right Mouse Button + correct directional key.
        /// </summary>
        private void CheckParryInput()
        {
            if (!Input.GetMouseButtonDown(1)) return; // Right click required

            // Check stamina
            float staminaCost = config != null ? config.ParryStaminaCost : 30f;
            float minStamina  = config != null ? config.MinStaminaToParry : 25f;

            if (staminaSystem != null && staminaSystem.CurrentStamina < minStamina)
            {
                // Not enough stamina — cannot parry
                return;
            }

            // Determine which direction key is held
            AttackDirection? heldDirection = GetHeldDirection();

            // Consume stamina regardless of success/failure
            if (staminaSystem != null)
                staminaSystem.SetStamina(staminaSystem.CurrentStamina - staminaCost);

            activeWindow.MarkAttempted();

            bool success = heldDirection.HasValue &&
                           heldDirection.Value == GetCorrectParryDirection(activeWindow.AttackDirection);

            if (success)
                OnParrySuccess();
            else
                OnParryFailure();

            EventManager.Instance.InvokeParryAttempted(success);
        }

        // ─── Parry Outcomes ───────────────────────────────────────────────────

        private void OnParrySuccess()
        {
            // Stun the active enemy
            GameObject enemy = Systems.Combat.CombatManager.Instance?.ActiveEnemy;
            if (enemy != null)
            {
                float stunDuration = config != null ? config.ParryStunDuration : 2f;
                var predatorAI = enemy.GetComponent<AI.Wildlife.PredatorAI>();
                if (predatorAI != null)
                    predatorAI.ApplyStun(stunDuration);

                EventManager.Instance.InvokeEnemyStunned(enemy, stunDuration);
            }
        }

        private void OnParryFailure()
        {
            // Apply stagger to player
            float staggerDuration = config != null ? config.ParryFailStaggerDuration : 0.8f;
            isStaggered = true;
            staggerTimer = staggerDuration;

            // Notify PlayerController to suppress input during stagger
            var controller = GetComponent<PlayerController>();
            if (controller != null)
                controller.SetCombatMode(true); // Keep combat mode active during stagger
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the direction key currently held by the player, or null if none.
        /// </summary>
        private AttackDirection? GetHeldDirection()
        {
            if (Input.GetKey(KeyCode.W)) return AttackDirection.Front;
            if (Input.GetKey(KeyCode.S)) return AttackDirection.Back;
            if (Input.GetKey(KeyCode.A)) return AttackDirection.Left;
            if (Input.GetKey(KeyCode.D)) return AttackDirection.Right;
            return null;
        }

        /// <summary>
        /// Returns the direction the player must hold to parry a given attack.
        /// Parry is directional: hold toward the attack to block it.
        ///   Front attack → hold W (into the attack)
        ///   Back attack  → hold S
        ///   Left attack  → hold A
        ///   Right attack → hold D
        /// </summary>
        private AttackDirection GetCorrectParryDirection(AttackDirection attackDir)
        {
            // Parry INTO the attack (opposite of dodge)
            switch (attackDir)
            {
                case AttackDirection.Front: return AttackDirection.Front;
                case AttackDirection.Back:  return AttackDirection.Back;
                case AttackDirection.Left:  return AttackDirection.Left;
                case AttackDirection.Right: return AttackDirection.Right;
                default: return AttackDirection.Front;
            }
        }

        private void TickStagger()
        {
            if (!isStaggered) return;
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0f)
                isStaggered = false;
        }
    }
}
