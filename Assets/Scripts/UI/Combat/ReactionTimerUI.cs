using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.ScriptableObjects.Combat;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Displays a countdown bar showing how much time the player has left
    /// to dodge or parry an incoming attack.
    ///
    /// The bar fills from full to empty during the reaction window.
    /// Color transitions from green → yellow → red as time runs out.
    /// </summary>
    public class ReactionTimerUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("UI References")]
        [Tooltip("Slider or Image (Filled) used as the timer bar")]
        [SerializeField] private Slider timerSlider;

        [Tooltip("Image component of the timer bar fill (for color changes)")]
        [SerializeField] private Image timerFillImage;

        [Header("Configuration")]
        [Tooltip("Combat configuration asset (for reaction window duration)")]
        [SerializeField] private CombatConfig config;

        [Header("Colors")]
        [SerializeField] private Color fullTimeColor  = new Color(0.2f, 0.9f, 0.2f);
        [SerializeField] private Color halfTimeColor  = new Color(1f,   0.8f, 0f);
        [SerializeField] private Color lowTimeColor   = new Color(1f,   0.2f, 0.2f);

        // ─── State ────────────────────────────────────────────────────────────
        private bool isActive = false;
        private float windowDuration = 0.5f;
        private float elapsed = 0f;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            SetVisible(false);
        }

        private void OnEnable()
        {
            EventManager.Instance.OnAttackTelegraphed += HandleAttackTelegraphed;
            EventManager.Instance.OnDodgeAttempted    += HandleDodgeAttempted;
            EventManager.Instance.OnCombatEnded       += HandleCombatEnded;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnAttackTelegraphed -= HandleAttackTelegraphed;
            EventManager.Instance.OnDodgeAttempted    -= HandleDodgeAttempted;
            EventManager.Instance.OnCombatEnded       -= HandleCombatEnded;
        }

        private void Update()
        {
            if (!isActive) return;

            elapsed += Time.unscaledDeltaTime;
            float remaining = Mathf.Clamp01(1f - (elapsed / windowDuration));

            UpdateBar(remaining);

            if (elapsed >= windowDuration)
            {
                SetVisible(false);
                isActive = false;
            }
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleAttackTelegraphed(ScriptableObjects.AI.AttackDirection direction, float telegraphDuration)
        {
            // Timer bar activates when the reaction window opens (after telegraph)
            // We delay activation by the telegraph duration
            StartCoroutine(ActivateAfterDelay(telegraphDuration));
        }

        private System.Collections.IEnumerator ActivateAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            windowDuration = config != null ? config.ReactionWindowDuration : 0.5f;
            elapsed = 0f;
            isActive = true;
            SetVisible(true);
        }

        private void HandleDodgeAttempted(ScriptableObjects.AI.AttackDirection input,
                                           ScriptableObjects.AI.AttackDirection attack,
                                           Systems.Combat.DodgeOutcome outcome)
        {
            // Hide timer immediately when player inputs
            SetVisible(false);
            isActive = false;
        }

        private void HandleCombatEnded()
        {
            SetVisible(false);
            isActive = false;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void UpdateBar(float normalizedValue)
        {
            if (timerSlider != null)
                timerSlider.value = normalizedValue;

            if (timerFillImage != null)
            {
                Color barColor;
                if (normalizedValue > 0.5f)
                    barColor = Color.Lerp(halfTimeColor, fullTimeColor, (normalizedValue - 0.5f) * 2f);
                else
                    barColor = Color.Lerp(lowTimeColor, halfTimeColor, normalizedValue * 2f);

                timerFillImage.color = barColor;
            }
        }

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
