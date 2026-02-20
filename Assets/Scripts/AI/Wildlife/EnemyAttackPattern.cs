using UnityEngine;
using System.Collections.Generic;
using Tsarkel.ScriptableObjects.AI;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Helper component that stores and provides attack patterns for an enemy.
    /// Works alongside EnemyAttackTelegraph to separate data from execution logic.
    /// Attach to the same GameObject as EnemyAttackTelegraph.
    /// </summary>
    public class EnemyAttackPattern : MonoBehaviour
    {
        [Header("Available Patterns")]
        [Tooltip("All attack patterns this enemy can use, ordered by preference")]
        [SerializeField] private List<AttackPatternData> patterns = new List<AttackPatternData>();

        [Header("Difficulty Gating")]
        [Tooltip("Current in-game days (set by WildlifeManager or TsunamiIntensityScaler)")]
        [SerializeField] private float currentDaysPassed = 0f;

        /// <summary>All patterns assigned to this enemy.</summary>
        public IReadOnlyList<AttackPatternData> Patterns => patterns;

        /// <summary>Current in-game days used for difficulty gating.</summary>
        public float CurrentDaysPassed
        {
            get => currentDaysPassed;
            set => currentDaysPassed = value;
        }

        /// <summary>
        /// Returns all patterns that are unlocked for the current day count.
        /// </summary>
        public List<AttackPatternData> GetUnlockedPatterns()
        {
            var result = new List<AttackPatternData>();
            foreach (var p in patterns)
            {
                if (p != null && p.IsValid && p.MinDayToUnlock <= currentDaysPassed)
                    result.Add(p);
            }
            return result;
        }

        /// <summary>
        /// Returns a random unlocked pattern using weighted selection.
        /// Returns null if no patterns are available.
        /// </summary>
        public AttackPatternData GetRandomPattern()
        {
            var unlocked = GetUnlockedPatterns();
            if (unlocked.Count == 0) return null;

            int totalWeight = 0;
            foreach (var p in unlocked) totalWeight += p.SelectionWeight;

            int roll = Random.Range(0, totalWeight);
            int accumulated = 0;
            foreach (var p in unlocked)
            {
                accumulated += p.SelectionWeight;
                if (roll < accumulated) return p;
            }

            return unlocked[0];
        }

        /// <summary>
        /// Adds a pattern at runtime (e.g., for boss phase transitions).
        /// </summary>
        public void AddPattern(AttackPatternData pattern)
        {
            if (pattern != null && !patterns.Contains(pattern))
                patterns.Add(pattern);
        }

        /// <summary>
        /// Removes a pattern at runtime.
        /// </summary>
        public void RemovePattern(AttackPatternData pattern)
        {
            patterns.Remove(pattern);
        }
    }
}
