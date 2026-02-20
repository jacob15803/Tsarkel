using UnityEngine;

namespace Tsarkel.ScriptableObjects.Combat
{
    /// <summary>
    /// Configuration asset for the directional combat system.
    /// Controls timing, damage multipliers, and difficulty parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Tsarkel/Combat/Combat Config")]
    public class CombatConfig : ScriptableObject
    {
        // ─────────────────────────────────────────────────────────────────────
        // Combat Mode
        // ─────────────────────────────────────────────────────────────────────

        [Header("Combat Mode")]
        [Tooltip("Time scale applied when combat mode is active (1 = normal, 0.5 = half speed)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float combatTimeScale = 0.7f;

        [Tooltip("Seconds to transition into combat time scale")]
        [SerializeField] private float timeScaleTransitionSpeed = 3f;

        [Tooltip("Whether to lock the camera onto the enemy during combat")]
        [SerializeField] private bool enableLockOn = true;

        [Tooltip("Distance at which combat mode automatically exits (enemy too far)")]
        [SerializeField] private float combatExitDistance = 20f;

        // ─────────────────────────────────────────────────────────────────────
        // Telegraph Timing
        // ─────────────────────────────────────────────────────────────────────

        [Header("Telegraph Timing")]
        [Tooltip("How long the enemy telegraphs its attack direction (seconds)")]
        [SerializeField] private float telegraphDuration = 1.0f;

        [Tooltip("How long the player has to react after the telegraph (seconds)")]
        [SerializeField] private float reactionWindowDuration = 0.5f;

        [Tooltip("Minimum telegraph duration (used by difficulty scaling)")]
        [SerializeField] private float minTelegraphDuration = 0.8f;

        [Tooltip("Maximum telegraph duration (used by difficulty scaling)")]
        [SerializeField] private float maxTelegraphDuration = 1.2f;

        [Tooltip("Minimum reaction window duration (used by difficulty scaling)")]
        [SerializeField] private float minReactionWindow = 0.4f;

        [Tooltip("Maximum reaction window duration (used by difficulty scaling)")]
        [SerializeField] private float maxReactionWindow = 0.6f;

        // ─────────────────────────────────────────────────────────────────────
        // Dodge Settings
        // ─────────────────────────────────────────────────────────────────────

        [Header("Dodge Settings")]
        [Tooltip("Damage multiplier for a perfect dodge (0 = no damage)")]
        [Range(0f, 1f)]
        [SerializeField] private float perfectDodgeDamageMultiplier = 0f;

        [Tooltip("Damage multiplier for a partial dodge (side dodge or late input)")]
        [Range(0f, 1f)]
        [SerializeField] private float partialDodgeDamageMultiplier = 0.5f;

        [Tooltip("Damage multiplier for a failed dodge (wrong direction or no input)")]
        [Range(1f, 3f)]
        [SerializeField] private float failedDodgeDamageMultiplier = 1f;

        [Tooltip("Duration of invincibility frames after a perfect dodge (seconds)")]
        [SerializeField] private float perfectDodgeInvincibilityDuration = 0.3f;

        [Tooltip("Stamina cost for a jump dodge (Space)")]
        [SerializeField] private float jumpDodgeStaminaCost = 15f;

        // ─────────────────────────────────────────────────────────────────────
        // Parry Settings
        // ─────────────────────────────────────────────────────────────────────

        [Header("Parry Settings")]
        [Tooltip("Duration of the parry timing window (seconds)")]
        [SerializeField] private float parryWindowDuration = 0.45f;

        [Tooltip("Stamina cost to attempt a parry")]
        [SerializeField] private float parryStaminaCost = 30f;

        [Tooltip("Minimum stamina required to attempt a parry")]
        [SerializeField] private float minStaminaToParry = 25f;

        [Tooltip("How long the enemy is stunned after a successful parry (seconds)")]
        [SerializeField] private float parryStunDuration = 2.0f;

        [Tooltip("Damage multiplier on parry failure (penalty for mistimed parry)")]
        [Range(1f, 3f)]
        [SerializeField] private float parryFailDamageMultiplier = 1.5f;

        [Tooltip("How long the player is staggered after a failed parry (seconds)")]
        [SerializeField] private float parryFailStaggerDuration = 0.8f;

        // ─────────────────────────────────────────────────────────────────────
        // Instinct Mode Settings
        // ─────────────────────────────────────────────────────────────────────

        [Header("Instinct Mode")]
        [Tooltip("Number of Instinct Mode charges per combat encounter")]
        [Range(1, 5)]
        [SerializeField] private int instinctChargesPerEncounter = 2;

        [Tooltip("Time scale while Instinct Mode is active")]
        [Range(0.1f, 0.5f)]
        [SerializeField] private float instinctTimeScale = 0.3f;

        [Tooltip("How long Instinct Mode lasts (seconds, in real time)")]
        [SerializeField] private float instinctDuration = 2.0f;

        [Tooltip("How many extra seconds of telegraph are shown during Instinct Mode")]
        [SerializeField] private float instinctEarlyTelegraphBonus = 0.5f;

        [Tooltip("Key used to activate Instinct Mode")]
        [SerializeField] private KeyCode instinctKey = KeyCode.Q;

        // ─────────────────────────────────────────────────────────────────────
        // Difficulty Scaling
        // ─────────────────────────────────────────────────────────────────────

        [Header("Difficulty Scaling")]
        [Tooltip("In-game day at which mid-game difficulty begins")]
        [SerializeField] private int midGameDay = 5;

        [Tooltip("In-game day at which late-game difficulty begins")]
        [SerializeField] private int lateGameDay = 12;

        // ─────────────────────────────────────────────────────────────────────
        // Public Accessors
        // ─────────────────────────────────────────────────────────────────────

        public float CombatTimeScale => combatTimeScale;
        public float TimeScaleTransitionSpeed => timeScaleTransitionSpeed;
        public bool EnableLockOn => enableLockOn;
        public float CombatExitDistance => combatExitDistance;

        public float TelegraphDuration => telegraphDuration;
        public float ReactionWindowDuration => reactionWindowDuration;
        public float MinTelegraphDuration => minTelegraphDuration;
        public float MaxTelegraphDuration => maxTelegraphDuration;
        public float MinReactionWindow => minReactionWindow;
        public float MaxReactionWindow => maxReactionWindow;

        public float PerfectDodgeDamageMultiplier => perfectDodgeDamageMultiplier;
        public float PartialDodgeDamageMultiplier => partialDodgeDamageMultiplier;
        public float FailedDodgeDamageMultiplier => failedDodgeDamageMultiplier;
        public float PerfectDodgeInvincibilityDuration => perfectDodgeInvincibilityDuration;
        public float JumpDodgeStaminaCost => jumpDodgeStaminaCost;

        public float ParryWindowDuration => parryWindowDuration;
        public float ParryStaminaCost => parryStaminaCost;
        public float MinStaminaToParry => minStaminaToParry;
        public float ParryStunDuration => parryStunDuration;
        public float ParryFailDamageMultiplier => parryFailDamageMultiplier;
        public float ParryFailStaggerDuration => parryFailStaggerDuration;

        public int InstinctChargesPerEncounter => instinctChargesPerEncounter;
        public float InstinctTimeScale => instinctTimeScale;
        public float InstinctDuration => instinctDuration;
        public float InstinctEarlyTelegraphBonus => instinctEarlyTelegraphBonus;
        public KeyCode InstinctKey => instinctKey;

        public int MidGameDay => midGameDay;
        public int LateGameDay => lateGameDay;

        /// <summary>
        /// Returns scaled telegraph duration based on current in-game day.
        /// </summary>
        public float GetScaledTelegraphDuration(float daysPassed)
        {
            float t = GetDifficultyT(daysPassed);
            return Mathf.Lerp(maxTelegraphDuration, minTelegraphDuration, t);
        }

        /// <summary>
        /// Returns scaled reaction window duration based on current in-game day.
        /// </summary>
        public float GetScaledReactionWindow(float daysPassed)
        {
            float t = GetDifficultyT(daysPassed);
            return Mathf.Lerp(maxReactionWindow, minReactionWindow, t);
        }

        /// <summary>
        /// Returns a 0-1 difficulty progression value based on days passed.
        /// 0 = early game, 1 = late game.
        /// </summary>
        private float GetDifficultyT(float daysPassed)
        {
            if (daysPassed <= midGameDay) return 0f;
            if (daysPassed >= lateGameDay) return 1f;
            return (daysPassed - midGameDay) / (float)(lateGameDay - midGameDay);
        }
    }
}
