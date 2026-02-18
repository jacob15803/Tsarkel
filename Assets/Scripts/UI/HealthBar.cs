using UnityEngine;
using UnityEngine.UI;
using Tsarkel.Managers;

namespace Tsarkel.UI
{
    /// <summary>
    /// UI component that displays player health bar.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Health bar fill image")]
        [SerializeField] private Image healthFill;
        
        [Tooltip("Health text (optional)")]
        [SerializeField] private Text healthText;
        
        [Header("Color Settings")]
        [Tooltip("Color when health is high (>70%)")]
        [SerializeField] private Color highHealthColor = Color.green;
        
        [Tooltip("Color when health is medium (30-70%)")]
        [SerializeField] private Color mediumHealthColor = Color.yellow;
        
        [Tooltip("Color when health is low (<30%)")]
        [SerializeField] private Color lowHealthColor = Color.red;
        
        [Tooltip("Smooth transition speed")]
        [SerializeField] private float transitionSpeed = 5f;
        
        private float targetFillAmount = 1f;
        private Color targetColor;
        
        private void OnEnable()
        {
            EventManager.Instance.OnPlayerHealthChanged += HandleHealthChanged;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnPlayerHealthChanged -= HandleHealthChanged;
        }
        
        private void Update()
        {
            // Smoothly update fill amount
            if (healthFill != null)
            {
                healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
                healthFill.color = Color.Lerp(healthFill.color, targetColor, Time.deltaTime * transitionSpeed);
            }
        }
        
        /// <summary>
        /// Handles health changed event.
        /// </summary>
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            targetFillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            
            // Determine color based on health percentage
            float healthPercentage = targetFillAmount;
            if (healthPercentage > 0.7f)
            {
                targetColor = highHealthColor;
            }
            else if (healthPercentage > 0.3f)
            {
                targetColor = mediumHealthColor;
            }
            else
            {
                targetColor = lowHealthColor;
            }
            
            // Update text if available
            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
            }
        }
    }
}
