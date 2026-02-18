using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.UI
{
    /// <summary>
    /// Central UI manager that coordinates all UI panels and components.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [Tooltip("Main HUD panel")]
        [SerializeField] private GameObject hudPanel;
        
        [Tooltip("Building menu panel")]
        [SerializeField] private GameObject buildingMenuPanel;
        
        [Tooltip("Pause menu panel")]
        [SerializeField] private GameObject pauseMenuPanel;
        
        [Tooltip("Death screen panel")]
        [SerializeField] private GameObject deathScreenPanel;
        
        private bool isPaused = false;
        
        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public bool IsPaused => isPaused;
        
        private void OnEnable()
        {
            EventManager.Instance.OnGamePaused += HandleGamePaused;
            EventManager.Instance.OnGameResumed += HandleGameResumed;
            EventManager.Instance.OnPlayerDeath += HandlePlayerDeath;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnGamePaused -= HandleGamePaused;
            EventManager.Instance.OnGameResumed -= HandleGameResumed;
            EventManager.Instance.OnPlayerDeath -= HandlePlayerDeath;
        }
        
        private void Update()
        {
            // Handle pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
        
        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
            
            EventManager.Instance.InvokeGamePaused();
        }
        
        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
            
            EventManager.Instance.InvokeGameResumed();
        }
        
        /// <summary>
        /// Handles game paused event.
        /// </summary>
        private void HandleGamePaused()
        {
            // Additional pause logic if needed
        }
        
        /// <summary>
        /// Handles game resumed event.
        /// </summary>
        private void HandleGameResumed()
        {
            // Additional resume logic if needed
        }
        
        /// <summary>
        /// Handles player death event.
        /// </summary>
        private void HandlePlayerDeath()
        {
            if (deathScreenPanel != null)
            {
                deathScreenPanel.SetActive(true);
            }
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        /// <summary>
        /// Shows the building menu.
        /// </summary>
        public void ShowBuildingMenu()
        {
            if (buildingMenuPanel != null)
            {
                buildingMenuPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hides the building menu.
        /// </summary>
        public void HideBuildingMenu()
        {
            if (buildingMenuPanel != null)
            {
                buildingMenuPanel.SetActive(false);
            }
        }
    }
}
