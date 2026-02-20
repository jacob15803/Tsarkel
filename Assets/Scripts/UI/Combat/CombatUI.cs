using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;
using Tsarkel.Systems.Combat;

namespace Tsarkel.UI.Combat
{
    /// <summary>
    /// Main coordinator for all combat UI panels.
    /// Shows/hides sub-panels based on combat state events.
    /// Attach to a Canvas GameObject that contains all combat UI children.
    /// </summary>
    public class CombatUI : MonoBehaviour
    {
        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Sub-Panels")]
        [Tooltip("Root panel shown during active combat")]
        [SerializeField] private GameObject combatPanel;

        [Tooltip("Directional attack indicator component")]
        [SerializeField] private AttackTelegraphUI telegraphUI;

        [Tooltip("Reaction window timer component")]
        [SerializeField] private ReactionTimerUI reactionTimerUI;

        [Tooltip("Dodge success/fail feedback component")]
        [SerializeField] private DodgeFeedbackUI dodgeFeedbackUI;

        [Tooltip("Parry timing indicator component")]
        [SerializeField] private ParryIndicatorUI parryIndicatorUI;

        [Tooltip("Instinct Mode charges indicator component")]
        [SerializeField] private InstinctModeUI instinctModeUI;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
        private void Start()
        {
            SetCombatPanelVisible(false);
        }

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

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleCombatStarted(GameObject enemy)
        {
            SetCombatPanelVisible(true);
        }

        private void HandleCombatEnded()
        {
            SetCombatPanelVisible(false);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void SetCombatPanelVisible(bool visible)
        {
            if (combatPanel != null)
                combatPanel.SetActive(visible);
        }
    }
}
