using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.ScriptableObjects.Combat;
using Tsarkel.Systems.Combat;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Attached to an enemy GameObject.
    /// Drives the telegraph → reaction window → attack execution pipeline.
    /// Notifies CombatManager to enter combat mode and fires events for the UI.
    /// </summary>
    public class EnemyAttackTelegraph : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Configuration")]
        [Tooltip("Combat configuration (timing, damage multipliers)")]
        [SerializeField] private CombatConfig combatConfig;

        [Header("Attack Patterns")]
        [Tooltip("All attack patterns available to this enemy")]
        [SerializeField] private List<AttackPatternData> availablePatterns = new List<AttackPatternData>();

        [Header("Visual Feedback")]
        [Tooltip("Renderer used to flash the enemy during telegraph (optional)")]
        [SerializeField] private Renderer telegraphRenderer;

        [Tooltip("Color shown on the attack side during telegraph")]
        [SerializeField] private Color telegraphColor = new Color(1f, 0.2f, 0.2f, 1f);

        [Tooltip("Normal material color (restored after telegraph)")]
        [SerializeField] private Color normalColor = Color.white;

        [Header("Audio")]
        [Tooltip("Sound played when telegraph begins")]
        [SerializeField] private AudioClip telegraphSound;

        [Tooltip("Sound played when attack lands")]
        [SerializeField] private AudioClip attackSound;

        // ─── State ────────────────────────────────────────────────────────────
        private bool isExecutingPattern = false;
        private Dictionary<AttackPatternData, float> patternCooldowns = new Dictionary<AttackPatternData, float>();
        private AudioSource audioSource;

        /// <summary>Whether this enemy is currently mid-attack pattern.</summary>
        public bool IsExecutingPattern => isExecutingPattern;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Update()
        {
            // Tick down pattern cooldowns
            var keys = new List<AttackPatternData>(patternCooldowns.Keys);
            foreach (var pattern in keys)
            {
                if (patternCooldowns[pattern] > 0f)
                    patternCooldowns[pattern] -= Time.deltaTime;
            }
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Selects and executes an attack pattern against the player.
        /// Called by PredatorAI when entering the Attack state.
        /// Automatically reads days from CombatDifficultyScaler if available.
        /// </summary>
        /// <param name="daysPassed">
        /// Current in-game days for difficulty scaling.
        /// Pass 0 to auto-read from CombatDifficultyScaler.
        /// </param>
        public void ExecuteAttack(float daysPassed = 0f)
        {
            if (isExecutingPattern) return;

            // Auto-read days from scaler if available
            float days = daysPassed;
            var scaler = Systems.Combat.CombatDifficultyScaler.Instance;
            if (scaler != null) days = scaler.DaysPassed;

            AttackPatternData pattern = SelectPattern(days);
            if (pattern == null || !pattern.IsValid) return;

            StartCoroutine(ExecutePatternCoroutine(pattern, days));
        }

        // ─── Pattern Selection ────────────────────────────────────────────────

        /// <summary>
        /// Picks a random available pattern weighted by SelectionWeight,
        /// respecting cooldowns and day-unlock requirements.
        /// </summary>
        private AttackPatternData SelectPattern(float daysPassed)
        {
            var candidates = new List<(AttackPatternData pattern, int weight)>();

            foreach (var pattern in availablePatterns)
            {
                if (pattern == null || !pattern.IsValid) continue;
                if (pattern.MinDayToUnlock > daysPassed) continue;
                if (patternCooldowns.TryGetValue(pattern, out float cd) && cd > 0f) continue;

                candidates.Add((pattern, pattern.SelectionWeight));
            }

            if (candidates.Count == 0) return null;

            // Weighted random selection
            int totalWeight = 0;
            foreach (var c in candidates) totalWeight += c.weight;

            int roll = Random.Range(0, totalWeight);
            int accumulated = 0;
            foreach (var c in candidates)
            {
                accumulated += c.weight;
                if (roll < accumulated) return c.pattern;
            }

            return candidates[0].pattern;
        }

        // ─── Pattern Execution Coroutine ──────────────────────────────────────

        /// <summary>
        /// Runs through each AttackStep in the pattern:
        ///   1. Optional delay
        ///   2. Telegraph phase (visual + event)
        ///   3. Reaction window (player input handled by DirectionalDodge)
        ///   4. Attack execution (damage resolution)
        /// </summary>
        private IEnumerator ExecutePatternCoroutine(AttackPatternData pattern, float daysPassed)
        {
            isExecutingPattern = true;

            // Enter combat mode
            CombatManager.Instance.EnterCombat(gameObject);

            // Start pattern cooldown
            patternCooldowns[pattern] = pattern.Cooldown;

            foreach (var step in pattern.Steps)
            {
                // Optional delay between steps
                if (step.delayBeforeStep > 0f)
                    yield return new WaitForSecondsRealtime(step.delayBeforeStep);

                // ── Telegraph Phase ──────────────────────────────────────────
                // Prefer CombatDifficultyScaler for live-scaled values
                var scaler = Systems.Combat.CombatDifficultyScaler.Instance;
                float telegraphDuration = scaler != null
                    ? scaler.CurrentTelegraphDuration
                    : (combatConfig != null ? combatConfig.GetScaledTelegraphDuration(daysPassed) : 1.0f);

                // Instinct Mode may extend the effective telegraph time for the player
                var instinct = FindObjectOfType<Player.Combat.InstinctMode>();
                if (instinct != null && instinct.IsActive)
                    telegraphDuration += instinct.GetEarlyTelegraphBonus();

                PlayTelegraphVisual(step.direction);
                PlaySound(telegraphSound);
                EventManager.Instance.InvokeAttackTelegraphed(step.direction, telegraphDuration);

                yield return new WaitForSecondsRealtime(telegraphDuration);

                // ── Reaction Window ──────────────────────────────────────────
                float reactionWindow = scaler != null
                    ? scaler.CurrentReactionWindow
                    : (combatConfig != null ? combatConfig.GetScaledReactionWindow(daysPassed) : 0.5f);

                // DirectionalDodge listens to OnAttackTelegraphed and opens its own window.
                // We wait the reaction window duration here (real time, unaffected by time scale).
                yield return new WaitForSecondsRealtime(reactionWindow);

                // ── Attack Execution ─────────────────────────────────────────
                StopTelegraphVisual();

                if (!step.isFakeTelegraph)
                {
                    ResolveAttack(step);
                    PlaySound(attackSound);
                }
            }

            isExecutingPattern = false;
        }

        // ─── Attack Resolution ────────────────────────────────────────────────

        /// <summary>
        /// Resolves damage for a single attack step.
        /// Queries DirectionalDodge on the player for the outcome.
        /// </summary>
        private void ResolveAttack(AttackStep step)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            var dodge = player.GetComponent<Player.Combat.DirectionalDodge>();
            var playerStats = player.GetComponent<Player.PlayerStats>();
            if (playerStats == null) return;

            // Determine base damage
            float baseDamage = step.overrideDamage > 0f
                ? step.overrideDamage
                : GetComponent<PredatorAI>()?.GetAttackDamage() ?? 10f;

            DodgeOutcome outcome = DodgeOutcome.Failed;
            float damageMultiplier = 1f;

            if (dodge != null)
            {
                outcome = dodge.ConsumeOutcome(step.direction);
            }

            if (combatConfig != null)
            {
                switch (outcome)
                {
                    case DodgeOutcome.Perfect:
                        damageMultiplier = combatConfig.PerfectDodgeDamageMultiplier;
                        break;
                    case DodgeOutcome.Partial:
                        damageMultiplier = combatConfig.PartialDodgeDamageMultiplier;
                        break;
                    case DodgeOutcome.Failed:
                        damageMultiplier = combatConfig.FailedDodgeDamageMultiplier;
                        break;
                }
            }

            float finalDamage = baseDamage * damageMultiplier;
            if (finalDamage > 0f)
            {
                playerStats.TakeDamage(finalDamage, gameObject.name);
                EventManager.Instance.InvokePredatorAttackedPlayer(gameObject, finalDamage);
            }
        }

        // ─── Visual Helpers ───────────────────────────────────────────────────

        private void PlayTelegraphVisual(AttackDirection direction)
        {
            if (telegraphRenderer == null) return;
            telegraphRenderer.material.color = telegraphColor;
        }

        private void StopTelegraphVisual()
        {
            if (telegraphRenderer == null) return;
            telegraphRenderer.material.color = normalColor;
        }

        private void PlaySound(AudioClip clip)
        {
            if (audioSource == null || clip == null) return;
            audioSource.PlayOneShot(clip);
        }
    }
}
