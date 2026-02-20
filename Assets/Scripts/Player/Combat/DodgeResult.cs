using UnityEngine;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Systems.Combat;

namespace Tsarkel.Player.Combat
{
    /// <summary>
    /// Stores and provides access to the result of the most recent dodge attempt.
    /// Attach to the Player GameObject alongside DirectionalDodge.
    ///
    /// Other systems (UI, audio, animation) can query this component to react
    /// to dodge outcomes without coupling directly to DirectionalDodge.
    /// </summary>
    public class DodgeResult : MonoBehaviour
    {
        // ─── Last Result ──────────────────────────────────────────────────────

        /// <summary>Outcome of the most recent dodge attempt.</summary>
        public DodgeOutcome LastOutcome { get; private set; } = DodgeOutcome.Failed;

        /// <summary>Attack direction of the most recent incoming attack.</summary>
        public AttackDirection LastAttackDirection { get; private set; } = AttackDirection.Front;

        /// <summary>Input direction the player pressed for the most recent dodge.</summary>
        public AttackDirection LastInputDirection { get; private set; } = AttackDirection.Back;

        /// <summary>Whether the last dodge was a perfect dodge.</summary>
        public bool WasPerfect => LastOutcome == DodgeOutcome.Perfect;

        /// <summary>Whether the last dodge was a partial dodge.</summary>
        public bool WasPartial => LastOutcome == DodgeOutcome.Partial;

        /// <summary>Whether the last dodge failed.</summary>
        public bool WasFailed => LastOutcome == DodgeOutcome.Failed;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────

        private void OnEnable()
        {
            Managers.EventManager.Instance.OnDodgeAttempted += HandleDodgeAttempted;
        }

        private void OnDisable()
        {
            Managers.EventManager.Instance.OnDodgeAttempted -= HandleDodgeAttempted;
        }

        // ─── Event Handler ────────────────────────────────────────────────────

        private void HandleDodgeAttempted(AttackDirection inputDir, AttackDirection attackDir, DodgeOutcome outcome)
        {
            LastInputDirection = inputDir;
            LastAttackDirection = attackDir;
            LastOutcome = outcome;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a human-readable description of the last dodge result.
        /// Useful for debug overlays.
        /// </summary>
        public string GetResultDescription()
        {
            return $"Attack: {LastAttackDirection} | Input: {LastInputDirection} | Outcome: {LastOutcome}";
        }
    }
}
