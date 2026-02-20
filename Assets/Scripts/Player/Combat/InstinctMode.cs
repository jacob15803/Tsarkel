using System.Collections;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Combat;
using Tsarkel.Systems.Combat;

namespace Tsarkel.Player.Combat
{
    /// <summary>
    /// Attached to the Player GameObject.
    /// Instinct Mode is a limited-use combat ability that:
    ///   - Further slows time (beyond combat mode slow)
    ///   - Shows attack direction earlier (early telegraph bonus)
    ///   - Highlights enemy weak points (optional visual)
    ///   - Lasts 2 seconds of real time
    ///   - Has a limited number of charges per combat encounter
    ///
    /// Activated by pressing the Instinct key (default: Q) during combat.
    /// </summary>
    public class InstinctMode : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Configuration")]
        [Tooltip("Combat configuration asset")]
        [SerializeField] private CombatConfig config;

        [Header("Visual Feedback")]
        [Tooltip("Optional post-process or overlay effect activated during Instinct Mode")]
        [SerializeField] private GameObject instinctOverlayEffect;

        // ─── State ────────────────────────────────────────────────────────────
        private int remainingCharges = 0;
        private bool isActive = false;
        private Coroutine activeCoroutine = null;

        /// <summary>Whether Instinct Mode is currently active.</summary>
        public bool IsActive => isActive;

        /// <summary>Remaining charges for this combat encounter.</summary>
        public int RemainingCharges => remainingCharges;

        /// <summary>Maximum charges per encounter (from config).</summary>
        public int MaxCharges => config != null ? config.InstinctChargesPerEncounter : 2;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void OnEnable()
        {
            EventManager.Instance.OnCombatStarted += HandleCombatStarted;
            EventManager.Instance.OnCombatEnded   += HandleCombatEnded;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnCombatStarted -= HandleCombatStarted;
            EventManager.Instance.OnCombatEnded   -= HandleCombatEnded;
        }

        private void Update()
        {
            if (!CombatManager.Instance.IsInCombat) return;
            if (isActive) return;
            if (remainingCharges <= 0) return;

            KeyCode activationKey = config != null ? config.InstinctKey : KeyCode.Q;
            if (Input.GetKeyDown(activationKey))
                Activate();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Activates Instinct Mode, consuming one charge.
        /// </summary>
        public void Activate()
        {
            if (isActive || remainingCharges <= 0) return;

            remainingCharges--;
            activeCoroutine = StartCoroutine(InstinctCoroutine());
        }

        /// <summary>
        /// Forcibly ends Instinct Mode (e.g., combat ended mid-activation).
        /// </summary>
        public void ForceEnd()
        {
            if (!isActive) return;
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
                activeCoroutine = null;
            }
            EndInstinct();
        }

        // ─── Coroutine ────────────────────────────────────────────────────────

        private IEnumerator InstinctCoroutine()
        {
            isActive = true;

            // Apply deeper time slow
            float instinctScale = config != null ? config.InstinctTimeScale : 0.3f;
            Time.timeScale = instinctScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Enable visual overlay
            if (instinctOverlayEffect != null)
                instinctOverlayEffect.SetActive(true);

            // Fire event (UI shows charges, telegraph shows early)
            EventManager.Instance.InvokeInstinctModeActivated(remainingCharges);

            // Wait for duration (real time — unaffected by time scale)
            float duration = config != null ? config.InstinctDuration : 2f;
            yield return new WaitForSecondsRealtime(duration);

            EndInstinct();
        }

        private void EndInstinct()
        {
            isActive = false;
            activeCoroutine = null;

            // Restore combat time scale
            float combatScale = config != null ? config.CombatTimeScale : 0.7f;
            Time.timeScale = CombatManager.Instance.IsInCombat ? combatScale : 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Disable visual overlay
            if (instinctOverlayEffect != null)
                instinctOverlayEffect.SetActive(false);

            EventManager.Instance.InvokeInstinctModeEnded();
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleCombatStarted(GameObject enemy)
        {
            // Restore charges at the start of each encounter
            remainingCharges = config != null ? config.InstinctChargesPerEncounter : 2;

            // End any lingering activation
            if (isActive) ForceEnd();
        }

        private void HandleCombatEnded()
        {
            if (isActive) ForceEnd();
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the effective telegraph duration bonus provided by Instinct Mode.
        /// Used by AttackTelegraphUI to show the direction earlier.
        /// </summary>
        public float GetEarlyTelegraphBonus()
        {
            if (!isActive || config == null) return 0f;
            return config.InstinctEarlyTelegraphBonus;
        }
    }
}
