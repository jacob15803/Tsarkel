using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Player.Combat;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Displays a directional arrow indicator when an enemy telegraphs an attack.
    /// The arrow points in the direction the attack is coming FROM.
    ///
    /// Setup: Assign four arrow Image objects (one per direction) in the Inspector.
    /// Each arrow should be positioned around a central crosshair.
    /// </summary>
    public class AttackTelegraphUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Direction Arrows")]
        [Tooltip("Arrow image shown when attack comes from the front (W direction)")]
        [SerializeField] private Image arrowFront;

        [Tooltip("Arrow image shown when attack comes from behind (S direction)")]
        [SerializeField] private Image arrowBack;

        [Tooltip("Arrow image shown when attack comes from the left (A direction)")]
        [SerializeField] private Image arrowLeft;

        [Tooltip("Arrow image shown when attack comes from the right (D direction)")]
        [SerializeField] private Image arrowRight;

        [Header("Colors")]
        [Tooltip("Color of the arrow during normal telegraph")]
        [SerializeField] private Color normalTelegraphColor = new Color(1f, 0.3f, 0.3f, 1f);

        [Tooltip("Color of the arrow when Instinct Mode is active (early telegraph)")]
        [SerializeField] private Color instinctTelegraphColor = new Color(0.3f, 0.8f, 1f, 1f);

        [Header("Animation")]
        [Tooltip("How fast the arrow pulses (scale oscillation)")]
        [SerializeField] private float pulseSpeed = 4f;

        [Tooltip("Pulse scale range (1 = no pulse)")]
        [SerializeField] private float pulseScale = 1.15f;

        // ─── State ────────────────────────────────────────────────────────────
        private Image activeArrow = null;
        private bool isShowing = false;
        private float showTimer = 0f;
        private bool instinctActive = false;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            HideAllArrows();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnAttackTelegraphed    += HandleAttackTelegraphed;
            EventManager.Instance.OnInstinctModeActivated += HandleInstinctActivated;
            EventManager.Instance.OnInstinctModeEnded     += HandleInstinctEnded;
            EventManager.Instance.OnCombatEnded           += HandleCombatEnded;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnAttackTelegraphed    -= HandleAttackTelegraphed;
            EventManager.Instance.OnInstinctModeActivated -= HandleInstinctActivated;
            EventManager.Instance.OnInstinctModeEnded     -= HandleInstinctEnded;
            EventManager.Instance.OnCombatEnded           -= HandleCombatEnded;
        }

        private void Update()
        {
            if (!isShowing || activeArrow == null) return;

            // Pulse animation
            float scale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * (pulseScale - 1f);
            activeArrow.transform.localScale = Vector3.one * scale;

            // Auto-hide after duration
            showTimer -= Time.unscaledDeltaTime;
            if (showTimer <= 0f)
                HideAllArrows();
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleAttackTelegraphed(AttackDirection direction, float telegraphDuration)
        {
            ShowArrow(direction, telegraphDuration);
        }

        private void HandleInstinctActivated(int charges)
        {
            instinctActive = true;
            // Recolor active arrow if showing
            if (activeArrow != null)
                activeArrow.color = instinctTelegraphColor;
        }

        private void HandleInstinctEnded()
        {
            instinctActive = false;
            if (activeArrow != null)
                activeArrow.color = normalTelegraphColor;
        }

        private void HandleCombatEnded()
        {
            HideAllArrows();
            instinctActive = false;
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void ShowArrow(AttackDirection direction, float duration)
        {
            HideAllArrows();

            activeArrow = GetArrowForDirection(direction);
            if (activeArrow == null) return;

            activeArrow.gameObject.SetActive(true);
            activeArrow.color = instinctActive ? instinctTelegraphColor : normalTelegraphColor;
            activeArrow.transform.localScale = Vector3.one;

            isShowing = true;
            showTimer = duration;
        }

        private void HideAllArrows()
        {
            isShowing = false;
            activeArrow = null;

            SetArrowActive(arrowFront, false);
            SetArrowActive(arrowBack,  false);
            SetArrowActive(arrowLeft,  false);
            SetArrowActive(arrowRight, false);
        }

        private Image GetArrowForDirection(AttackDirection direction)
        {
            switch (direction)
            {
                case AttackDirection.Front: return arrowFront;
                case AttackDirection.Back:  return arrowBack;
                case AttackDirection.Left:  return arrowLeft;
                case AttackDirection.Right: return arrowRight;
                default: return null;
            }
        }

        private void SetArrowActive(Image arrow, bool active)
        {
            if (arrow != null) arrow.gameObject.SetActive(active);
        }
    }
}
