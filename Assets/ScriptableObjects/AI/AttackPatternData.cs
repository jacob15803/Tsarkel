using UnityEngine;
using System.Collections.Generic;

namespace Tsarkel.ScriptableObjects.AI
{
    /// <summary>
    /// Defines the attack direction enum shared across combat systems.
    /// </summary>
    public enum AttackDirection
    {
        /// <summary>Enemy attacks from the front — player must dodge backward (S).</summary>
        Front,
        /// <summary>Enemy attacks from behind — player must dodge forward (W).</summary>
        Back,
        /// <summary>Enemy attacks from the left — player must dodge right (D).</summary>
        Left,
        /// <summary>Enemy attacks from the right — player must dodge left (A).</summary>
        Right
    }

    /// <summary>
    /// A single attack step within a pattern.
    /// </summary>
    [System.Serializable]
    public class AttackStep
    {
        [Tooltip("Direction this attack comes from")]
        public AttackDirection direction = AttackDirection.Front;

        [Tooltip("Delay before this step begins (seconds). 0 = immediately after previous step resolves.")]
        [Range(0f, 3f)]
        public float delayBeforeStep = 0.2f;

        [Tooltip("Whether this is a fake telegraph (shows direction but deals no damage — punishes parry attempts)")]
        public bool isFakeTelegraph = false;

        [Tooltip("Damage dealt by this step (0 = use predator base damage)")]
        public float overrideDamage = 0f;
    }

    /// <summary>
    /// ScriptableObject defining a named attack pattern for an enemy.
    /// Patterns can be single attacks or multi-step combos.
    /// </summary>
    [CreateAssetMenu(fileName = "AttackPatternData", menuName = "Tsarkel/AI/Attack Pattern Data")]
    public class AttackPatternData : ScriptableObject
    {
        [Header("Pattern Identity")]
        [Tooltip("Readable name for this pattern (e.g., 'Lunge', 'Combo Left-Right')")]
        [SerializeField] private string patternName = "New Pattern";

        [Tooltip("Minimum in-game day before this pattern can be used (difficulty gating)")]
        [SerializeField] private int minDayToUnlock = 0;

        [Tooltip("Relative weight for random selection among available patterns (higher = more frequent)")]
        [Range(1, 10)]
        [SerializeField] private int selectionWeight = 5;

        [Header("Attack Steps")]
        [Tooltip("Ordered list of attack steps in this pattern")]
        [SerializeField] private List<AttackStep> steps = new List<AttackStep>();

        [Header("Cooldown")]
        [Tooltip("Minimum seconds before this pattern can be used again")]
        [SerializeField] private float cooldown = 3f;

        // ─── Public Accessors ─────────────────────────────────────────────────

        public string PatternName => patternName;
        public int MinDayToUnlock => minDayToUnlock;
        public int SelectionWeight => selectionWeight;
        public IReadOnlyList<AttackStep> Steps => steps;
        public float Cooldown => cooldown;

        /// <summary>
        /// Returns true if this pattern has at least one step.
        /// </summary>
        public bool IsValid => steps != null && steps.Count > 0;
    }
}
