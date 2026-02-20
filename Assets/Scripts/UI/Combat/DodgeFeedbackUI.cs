using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Systems.Combat;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Displays brief visual feedback after a dodge attempt.
    ///
    ///   Perfect Dodge → Green "PERFECT!" text
    ///   Partial Dodge → Yellow "PARTIAL" text
    ///   Failed Dodge  → Red "MISS" text
    ///
    /// Text fades out automatically after a short duration.
    /// </summary>
    public class DodgeFeedbackUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("UI References")]
        [Tooltip("Text component for dodge feedback (TextMeshPro recommended)")]
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Tooltip("Optional background panel behind the text")]
        [SerializeField] private Image backgroundPanel;

        [Header("Feedback Settings")]
        [Tooltip("How long the feedback text is visible (seconds)")]
        [SerializeField] private float displayDuration = 0.8f;

        [Tooltip("How fast the text fades out")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Colors")]
        [SerializeField] private Color perfectColor = new Color(0.2f, 1f, 0.3f);
        [SerializeField] private Color partialColor = new Color(1f,   0.85f, 0f);
        [SerializeField] private Color failedColor  = new Color(1f,   0.2f, 0.2f);

        [Header("Text Labels")]
        [SerializeField] private string perfectLabel = "PERFECT!";
        [SerializeField] private string partialLabel = "PARTIAL";
        [SerializeField] private string failedLabel  = "MISS";

        // ─── State ────────────────────────────────────────────────────────────
        private Coroutine activeCoroutine = null;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            SetVisible(false);
        }

        private void OnEnable()
        {
            EventManager.Instance.OnDodgeAttempted += HandleDodgeAttempted;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnDodgeAttempted -= HandleDodgeAttempted;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleDodgeAttempted(AttackDirection input, AttackDirection attack, DodgeOutcome outcome)
        {
            if (activeCoroutine != null)
                StopCoroutine(activeCoroutine);

            activeCoroutine = StartCoroutine(ShowFeedback(outcome));
        }

        // ─── Coroutine ────────────────────────────────────────────────────────

        private IEnumerator ShowFeedback(DodgeOutcome outcome)
        {
            // Configure text
            string label;
            Color color;

            switch (outcome)
            {
                case DodgeOutcome.Perfect:
                    label = perfectLabel;
                    color = perfectColor;
                    break;
                case DodgeOutcome.Partial:
                    label = partialLabel;
                    color = partialColor;
                    break;
                default:
                    label = failedLabel;
                    color = failedColor;
                    break;
            }

            if (feedbackText != null)
            {
                feedbackText.text  = label;
                feedbackText.color = color;
            }

            SetVisible(true);

            // Hold
            yield return new WaitForSecondsRealtime(displayDuration);

            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);

                if (feedbackText != null)
                {
                    Color c = feedbackText.color;
                    c.a = alpha;
                    feedbackText.color = c;
                }

                yield return null;
            }

            SetVisible(false);
            activeCoroutine = null;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
