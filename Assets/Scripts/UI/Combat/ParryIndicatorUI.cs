using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Player.Combat;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Displays the parry timing window as a shrinking bar.
    /// Also shows success/failure feedback after a parry attempt.
    ///
    /// Setup: Assign a Slider for the timing bar and optional text for feedback.
    /// The bar shrinks from full to empty during the parry window.
    /// A "sweet spot" zone can be shown to indicate the ideal parry moment.
    /// </summary>
    public class ParryIndicatorUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Timing Bar")]
        [Tooltip("Slider representing the parry window (value 1 = open, 0 = closed)")]
        [SerializeField] private Slider parrySlider;

        [Tooltip("Fill image of the parry slider (for color changes)")]
        [SerializeField] private Image parryFillImage;

        [Header("Feedback Text")]
        [Tooltip("Text shown after a parry attempt (PARRY! or FAILED)")]
        [SerializeField] private Text parryFeedbackText;

        [Header("Colors")]
        [SerializeField] private Color openWindowColor   = new Color(0.9f, 0.9f, 0.2f);
        [SerializeField] private Color closingWindowColor = new Color(1f,  0.4f, 0f);
        [SerializeField] private Color successColor      = new Color(0.2f, 1f,   0.3f);
        [SerializeField] private Color failureColor      = new Color(1f,   0.2f, 0.2f);

        [Header("Feedback Settings")]
        [SerializeField] private float feedbackDisplayDuration = 0.7f;
        [SerializeField] private string successLabel = "PARRY!";
        [SerializeField] private string failureLabel = "FAILED";

        // ─── Dependencies ─────────────────────────────────────────────────────
        [Header("Dependencies")]
        [Tooltip("ParrySystem component on the player")]
        [SerializeField] private ParrySystem parrySystem;

        // ─── State ────────────────────────────────────────────────────────────
        private bool isShowingWindow = false;
        private Coroutine feedbackCoroutine = null;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            SetBarVisible(false);
            SetFeedbackVisible(false);

            if (parrySystem == null)
                parrySystem = FindObjectOfType<ParrySystem>();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnAttackTelegraphed += HandleAttackTelegraphed;
            EventManager.Instance.OnParryAttempted    += HandleParryAttempted;
            EventManager.Instance.OnCombatEnded       += HandleCombatEnded;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnAttackTelegraphed -= HandleAttackTelegraphed;
            EventManager.Instance.OnParryAttempted    -= HandleParryAttempted;
            EventManager.Instance.OnCombatEnded       -= HandleCombatEnded;
        }

        private void Update()
        {
            if (!isShowingWindow || parrySystem == null) return;

            if (!parrySystem.HasActiveWindow)
            {
                SetBarVisible(false);
                isShowingWindow = false;
                return;
            }

            // Update bar based on window progress (1 = open, 0 = closed)
            float remaining = 1f - parrySystem.WindowProgress;
            UpdateBar(remaining);
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleAttackTelegraphed(AttackDirection direction, float telegraphDuration)
        {
            // Show bar when reaction window opens
            StartCoroutine(ShowBarAfterDelay(telegraphDuration));
        }

        private System.Collections.IEnumerator ShowBarAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            isShowingWindow = true;
            SetBarVisible(true);
        }

        private void HandleParryAttempted(bool success)
        {
            SetBarVisible(false);
            isShowingWindow = false;

            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);

            feedbackCoroutine = StartCoroutine(ShowParryFeedback(success));
        }

        private void HandleCombatEnded()
        {
            SetBarVisible(false);
            SetFeedbackVisible(false);
            isShowingWindow = false;
        }

        // ─── Coroutine ────────────────────────────────────────────────────────

        private IEnumerator ShowParryFeedback(bool success)
        {
            if (parryFeedbackText != null)
            {
                parryFeedbackText.text  = success ? successLabel : failureLabel;
                parryFeedbackText.color = success ? successColor : failureColor;
            }

            SetFeedbackVisible(true);
            yield return new WaitForSecondsRealtime(feedbackDisplayDuration);
            SetFeedbackVisible(false);
            feedbackCoroutine = null;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void UpdateBar(float normalizedValue)
        {
            if (parrySlider != null)
                parrySlider.value = normalizedValue;

            if (parryFillImage != null)
                parryFillImage.color = Color.Lerp(closingWindowColor, openWindowColor, normalizedValue);
        }

        private void SetBarVisible(bool visible)
        {
            if (parrySlider != null)
                parrySlider.gameObject.SetActive(visible);
        }

        private void SetFeedbackVisible(bool visible)
        {
            if (parryFeedbackText != null)
                parryFeedbackText.gameObject.SetActive(visible);
        }
    }
}
