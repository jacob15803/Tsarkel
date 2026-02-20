using UnityEngine;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.ScriptableObjects.Combat;

namespace Tsarkel.Player.Combat
{
    /// <summary>
    /// Tracks the parry timing window for a single incoming attack.
    /// Created and managed by ParrySystem.
    ///
    /// A parry window opens when an attack is telegraphed and closes
    /// after the configured parry window duration.
    /// </summary>
    public class ParryWindow
    {
        // ─── State ────────────────────────────────────────────────────────────

        /// <summary>The attack direction this window corresponds to.</summary>
        public AttackDirection AttackDirection { get; private set; }

        /// <summary>How long the parry window remains open (seconds, real time).</summary>
        public float Duration { get; private set; }

        /// <summary>Elapsed real time since the window opened.</summary>
        public float Elapsed { get; private set; }

        /// <summary>Whether the parry window is still open.</summary>
        public bool IsOpen => Elapsed < Duration;

        /// <summary>Normalised progress through the window (0 = just opened, 1 = closed).</summary>
        public float Progress => Duration > 0f ? Mathf.Clamp01(Elapsed / Duration) : 1f;

        /// <summary>Whether a parry attempt has already been made in this window.</summary>
        public bool WasAttempted { get; private set; }

        // ─── Constructor ──────────────────────────────────────────────────────

        public ParryWindow(AttackDirection direction, float duration)
        {
            AttackDirection = direction;
            Duration = duration;
            Elapsed = 0f;
            WasAttempted = false;
        }

        // ─── Update ───────────────────────────────────────────────────────────

        /// <summary>
        /// Advances the window timer. Call once per frame with unscaled delta time.
        /// </summary>
        public void Tick(float unscaledDeltaTime)
        {
            if (!IsOpen) return;
            Elapsed += unscaledDeltaTime;
        }

        /// <summary>
        /// Marks that a parry attempt was made in this window.
        /// </summary>
        public void MarkAttempted()
        {
            WasAttempted = true;
        }
    }
}
