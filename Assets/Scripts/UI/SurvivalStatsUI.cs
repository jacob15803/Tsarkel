using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;

namespace Tsarkel.UI
{
    /// <summary>
    /// UI component that displays hunger and hydration stats.
    /// </summary>
    public class SurvivalStatsUI : MonoBehaviour
    {
        [Header("Hunger UI")]
        [Tooltip("Hunger fill image")]
        [SerializeField] private Image hungerFill;
        
        [Tooltip("Hunger text (optional)")]
        [SerializeField] private Text hungerText;
        
        [Tooltip("Hunger icon (optional)")]
        [SerializeField] private Image hungerIcon;
        
        [Header("Hydration UI")]
        [Tooltip("Hydration fill image")]
        [SerializeField] private Image hydrationFill;
        
        [Tooltip("Hydration text (optional)")]
        [SerializeField] private Text hydrationText;
        
        [Tooltip("Hydration icon (optional)")]
        [SerializeField] private Image hydrationIcon;
        
        [Header("Color Settings")]
        [Tooltip("Normal color")]
        [SerializeField] private Color normalColor = Color.white;
        
        [Tooltip("Warning color (low values)")]
        [SerializeField] private Color warningColor = Color.yellow;
        
        [Tooltip("Critical color (very low values)")]
        [SerializeField] private Color criticalColor = Color.red;
        
        [Tooltip("Smooth transition speed")]
        [SerializeField] private float transitionSpeed = 5f;
        
        private float targetHungerFill = 1f;
        private float targetHydrationFill = 1f;
        private Color targetHungerColor;
        private Color targetHydrationColor;
        
        private void OnEnable()
        {
            EventManager.Instance.OnPlayerHungerChanged += HandleHungerChanged;
            EventManager.Instance.OnPlayerHydrationChanged += HandleHydrationChanged;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnPlayerHungerChanged -= HandleHungerChanged;
            EventManager.Instance.OnPlayerHydrationChanged -= HandleHydrationChanged;
        }
        
        private void Update()
        {
            // Smoothly update fill amounts
            if (hungerFill != null)
            {
                hungerFill.fillAmount = Mathf.Lerp(hungerFill.fillAmount, targetHungerFill, Time.deltaTime * transitionSpeed);
                hungerFill.color = Color.Lerp(hungerFill.color, targetHungerColor, Time.deltaTime * transitionSpeed);
            }
            
            if (hydrationFill != null)
            {
                hydrationFill.fillAmount = Mathf.Lerp(hydrationFill.fillAmount, targetHydrationFill, Time.deltaTime * transitionSpeed);
                hydrationFill.color = Color.Lerp(hydrationFill.color, targetHydrationColor, Time.deltaTime * transitionSpeed);
            }
        }
        
        /// <summary>
        /// Handles hunger changed event.
        /// </summary>
        private void HandleHungerChanged(float currentHunger, float maxHunger)
        {
            targetHungerFill = maxHunger > 0 ? currentHunger / maxHunger : 0f;
            
            // Determine color based on hunger percentage
            float hungerPercentage = targetHungerFill;
            if (hungerPercentage > 0.5f)
            {
                targetHungerColor = normalColor;
            }
            else if (hungerPercentage > 0.2f)
            {
                targetHungerColor = warningColor;
            }
            else
            {
                targetHungerColor = criticalColor;
            }
            
            // Update text if available
            if (hungerText != null)
            {
                hungerText.text = $"{Mathf.RoundToInt(currentHunger)}%";
            }
        }
        
        /// <summary>
        /// Handles hydration changed event.
        /// </summary>
        private void HandleHydrationChanged(float currentHydration, float maxHydration)
        {
            targetHydrationFill = maxHydration > 0 ? currentHydration / maxHydration : 0f;
            
            // Determine color based on hydration percentage
            float hydrationPercentage = targetHydrationFill;
            if (hydrationPercentage > 0.5f)
            {
                targetHydrationColor = normalColor;
            }
            else if (hydrationPercentage > 0.2f)
            {
                targetHydrationColor = warningColor;
            }
            else
            {
                targetHydrationColor = criticalColor;
            }
            
            // Update text if available
            if (hydrationText != null)
            {
                hydrationText.text = $"{Mathf.RoundToInt(currentHydration)}%";
            }
        }
    }
}
