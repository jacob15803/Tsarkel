using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Systems.Tsunami
{
    /// <summary>
    /// Handles visual and audio warnings during tsunami warning phase.
    /// </summary>
    public class TsunamiWarning : MonoBehaviour
    {
        [Header("Audio")]
        [Tooltip("Audio source for warning sounds")]
        [SerializeField] private AudioSource audioSource;
        
        [Tooltip("Warning sound clip")]
        [SerializeField] private AudioClip warningSound;
        
        [Tooltip("Volume for warning sound")]
        [SerializeField] private float warningVolume = 0.7f;
        
        [Header("Visual Effects")]
        [Tooltip("Particle system for warning effects")]
        [SerializeField] private ParticleSystem warningParticles;
        
        [Tooltip("UI warning panel (optional)")]
        [SerializeField] private GameObject warningUIPanel;
        
        [Header("Settings")]
        [Tooltip("Whether to play warning sound")]
        [SerializeField] private bool playWarningSound = true;
        
        [Tooltip("Whether to show visual effects")]
        [SerializeField] private bool showVisualEffects = true;
        
        private bool isWarningActive = false;
        
        private void OnEnable()
        {
            EventManager.Instance.OnTsunamiWarning += HandleTsunamiWarning;
            EventManager.Instance.OnTsunamiWave += HandleTsunamiWave;
            EventManager.Instance.OnTsunamiWaveEnd += HandleTsunamiWaveEnd;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnTsunamiWarning -= HandleTsunamiWarning;
            EventManager.Instance.OnTsunamiWave -= HandleTsunamiWave;
            EventManager.Instance.OnTsunamiWaveEnd -= HandleTsunamiWaveEnd;
        }
        
        /// <summary>
        /// Handles tsunami warning event.
        /// </summary>
        private void HandleTsunamiWarning(float warningDuration)
        {
            isWarningActive = true;
            
            if (playWarningSound && audioSource != null && warningSound != null)
            {
                audioSource.clip = warningSound;
                audioSource.volume = warningVolume;
                audioSource.Play();
            }
            
            if (showVisualEffects)
            {
                if (warningParticles != null && !warningParticles.isPlaying)
                {
                    warningParticles.Play();
                }
                
                if (warningUIPanel != null)
                {
                    warningUIPanel.SetActive(true);
                }
            }
        }
        
        /// <summary>
        /// Handles tsunami wave event (warning ends, wave begins).
        /// </summary>
        private void HandleTsunamiWave(float waveIntensity)
        {
            isWarningActive = false;
            
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            if (warningParticles != null && warningParticles.isPlaying)
            {
                warningParticles.Stop();
            }
        }
        
        /// <summary>
        /// Handles tsunami wave end event.
        /// </summary>
        private void HandleTsunamiWaveEnd()
        {
            if (warningUIPanel != null)
            {
                warningUIPanel.SetActive(false);
            }
        }
    }
}
