using UnityEngine;
using Tsarkel.ScriptableObjects.Combat;

namespace Tsarkel.Systems.Combat
{
    /// <summary>
    /// Attached to the Player GameObject.
    /// Manages the player's combat stance: locks movement, enables dodge input,
    /// and optionally rotates the player to face the active enemy.
    /// </summary>
    public class CombatStance : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Combat configuration asset")]
        [SerializeField] private CombatConfig config;

        [Header("Lock-On Settings")]
        [Tooltip("How fast the player rotates to face the enemy during lock-on")]
        [SerializeField] private float lockOnRotationSpeed = 8f;

        // ─── State ────────────────────────────────────────────────────────────
        private bool isInCombatStance = false;
        private GameObject lockedEnemy;

        /// <summary>Whether the player is currently in combat stance.</summary>
        public bool IsInCombatStance => isInCombatStance;

        /// <summary>The enemy the player is locked onto (null if none).</summary>
        public GameObject LockedEnemy => lockedEnemy;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Update()
        {
            if (!isInCombatStance) return;

            HandleLockOnRotation();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Activates combat stance and optionally locks onto the given enemy.
        /// </summary>
        public void EnterCombatStance(GameObject enemy)
        {
            isInCombatStance = true;
            lockedEnemy = enemy;

            // Notify PlayerController to suppress normal movement input
            var controller = GetComponent<Player.PlayerController>();
            if (controller != null)
                controller.SetCombatMode(true);
        }

        /// <summary>
        /// Deactivates combat stance and releases lock-on.
        /// </summary>
        public void ExitCombatStance()
        {
            isInCombatStance = false;
            lockedEnemy = null;

            var controller = GetComponent<Player.PlayerController>();
            if (controller != null)
                controller.SetCombatMode(false);
        }

        // ─── Private Helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Smoothly rotates the player to face the locked enemy (Y-axis only).
        /// </summary>
        private void HandleLockOnRotation()
        {
            if (lockedEnemy == null) return;
            if (config == null || !config.EnableLockOn) return;

            Vector3 direction = lockedEnemy.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                lockOnRotationSpeed * Time.unscaledDeltaTime);
        }
    }
}
