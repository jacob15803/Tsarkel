using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;

namespace Tsarkel.UI
{
    /// <summary>
    /// UI component that displays player stamina bar.
    /// </summary>
    public class StaminaBar : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Stamina bar fill image")]
        [SerializeField] private Image staminaFill;
        
        [Tooltip("Stamina text (optional)")]
        [SerializeField] private Text staminaText;
        
        [Header("Color Settings")]
        [Tooltip("Color when stamina is high")]
        [SerializeField] private Color highStaminaColor = Color.cyan;
        
        [Tooltip("Color when stamina is low")]
        [SerializeField] private Color lowStaminaColor = Color.red;
        
        [Tooltip("Smooth transition speed")]
        [SerializeField] private float transitionSpeed = 5f;
        
        private float targetFillAmount = 1f;
        private Color targetColor;
        
        private void OnEnable()
        {
            EventManager.Instance.OnPlayerStaminaChanged += HandleStaminaChanged;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnPlayerStaminaChanged -= HandleStaminaChanged;
        }
        
        private void Update()
        {
            // Smoothly update fill amount
            if (staminaFill != null)
            {
                staminaFill.fillAmount = Mathf.Lerp(staminaFill.fillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
                staminaFill.color = Color.Lerp(staminaFill.color, targetColor, Time.deltaTime * transitionSpeed);
            }
        }
        
        /// <summary>
        /// Handles stamina changed event.
        /// </summary>
        private void HandleStaminaChanged(float currentStamina, float maxStamina)
        {
            targetFillAmount = maxStamina > 0 ? currentStamina / maxStamina : 0f;
            
            // Determine color based on stamina percentage
            float staminaPercentage = targetFillAmount;
            targetColor = Color.Lerp(lowStaminaColor, highStaminaColor, staminaPercentage);
            
            // Update text if available
            if (staminaText != null)
            {
                staminaText.text = $"{Mathf.RoundToInt(currentStamina)} / {Mathf.RoundToInt(maxStamina)}";
            }
        }
    }
}
