using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tsarkel.Managers;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Displays the Instinct Mode charge count and activation feedback.
    ///
    /// Shows:
    ///   - Charge icons (filled/empty) indicating remaining uses
    ///   - "INSTINCT" activation flash when mode is triggered
    ///   - Dimmed state when no charges remain
    ///
    /// Setup: Assign charge icon Images and optional activation text.
    /// </summary>
    public class InstinctModeUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Charge Icons")]
        [Tooltip("Array of charge icon Images (one per max charge). Assign in order.")]
        [SerializeField] private Image[] chargeIcons;

        [Header("Icon Colors")]
        [Tooltip("Color when a charge is available")]
        [SerializeField] private Color chargeAvailableColor = new Color(0.3f, 0.8f, 1f, 1f);

        [Tooltip("Color when a charge has been used")]
        [SerializeField] private Color chargeUsedColor = new Color(0.2f, 0.2f, 0.3f, 0.5f);

        [Header("Activation Feedback")]
        [Tooltip("Text shown briefly when Instinct Mode activates")]
        [SerializeField] private TextMeshProUGUI activationText;

        [Tooltip("How long the activation text is shown (seconds)")]
        [SerializeField] private float activationTextDuration = 0.6f;

        [Tooltip("Optional screen overlay image shown during Instinct Mode")]
        [SerializeField] private Image instinctScreenOverlay;

        [Header("Overlay Settings")]
        [Tooltip("Alpha of the screen overlay during Instinct Mode")]
        [Range(0f, 0.5f)]
        [SerializeField] private float overlayAlpha = 0.15f;

        // ─── State ────────────────────────────────────────────────────────────
        private Coroutine activationCoroutine = null;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            SetActivationTextVisible(false);
            SetOverlayVisible(false);
            UpdateChargeIcons(0, 0); // Start empty; will update on combat start
        }

        private void OnEnable()
        {
            EventManager.Instance.OnCombatStarted        += HandleCombatStarted;
            EventManager.Instance.OnCombatEnded          += HandleCombatEnded;
            EventManager.Instance.OnInstinctModeActivated += HandleInstinctActivated;
            EventManager.Instance.OnInstinctModeEnded     += HandleInstinctEnded;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnCombatStarted        -= HandleCombatStarted;
            EventManager.Instance.OnCombatEnded          -= HandleCombatEnded;
            EventManager.Instance.OnInstinctModeActivated -= HandleInstinctActivated;
            EventManager.Instance.OnInstinctModeEnded     -= HandleInstinctEnded;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleCombatStarted(GameObject enemy)
        {
            // Find InstinctMode on player to get max charges
            var instinct = FindObjectOfType<Player.Combat.InstinctMode>();
            if (instinct != null)
                UpdateChargeIcons(instinct.RemainingCharges, instinct.MaxCharges);
        }

        private void HandleCombatEnded()
        {
            SetActivationTextVisible(false);
            SetOverlayVisible(false);
        }

        private void HandleInstinctActivated(int remainingCharges)
        {
            var instinct = FindObjectOfType<Player.Combat.InstinctMode>();
            int maxCharges = instinct != null ? instinct.MaxCharges : chargeIcons.Length;
            UpdateChargeIcons(remainingCharges, maxCharges);

            if (activationCoroutine != null)
                StopCoroutine(activationCoroutine);
            activationCoroutine = StartCoroutine(ShowActivationFeedback());

            SetOverlayVisible(true);
        }

        private void HandleInstinctEnded()
        {
            SetOverlayVisible(false);
        }

        // ─── Coroutine ────────────────────────────────────────────────────────

        private IEnumerator ShowActivationFeedback()
        {
            SetActivationTextVisible(true);
            yield return new WaitForSecondsRealtime(activationTextDuration);
            SetActivationTextVisible(false);
            activationCoroutine = null;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Updates charge icons: first <paramref name="remaining"/> icons are lit,
        /// the rest are dimmed.
        /// </summary>
        private void UpdateChargeIcons(int remaining, int max)
        {
            if (chargeIcons == null) return;

            for (int i = 0; i < chargeIcons.Length; i++)
            {
                if (chargeIcons[i] == null) continue;

                bool withinMax = i < max;
                chargeIcons[i].gameObject.SetActive(withinMax);

                if (withinMax)
                    chargeIcons[i].color = i < remaining ? chargeAvailableColor : chargeUsedColor;
            }
        }

        private void SetActivationTextVisible(bool visible)
        {
            if (activationText != null)
                activationText.gameObject.SetActive(visible);
        }

        private void SetOverlayVisible(bool visible)
        {
            if (instinctScreenOverlay == null) return;

            instinctScreenOverlay.gameObject.SetActive(visible);
            if (visible)
            {
                Color c = instinctScreenOverlay.color;
                c.a = overlayAlpha;
                instinctScreenOverlay.color = c;
            }
        }
    }
}
