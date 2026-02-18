using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.Player;
using Tsarkel.Systems.Tsunami;
using Tsarkel.Systems.Building;

namespace Tsarkel.Managers
{
    /// <summary>
    /// Main game manager that coordinates all systems and manages game state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("System References")]
        [Tooltip("Event manager reference")]
        [SerializeField] private EventManager eventManager;
        
        [Tooltip("Player stats reference")]
        [SerializeField] private PlayerStats playerStats;
        
        [Tooltip("Tsunami manager reference")]
        [SerializeField] private TsunamiManager tsunamiManager;
        
        [Tooltip("Building manager reference")]
        [SerializeField] private BuildingManager buildingManager;
        
        [Header("Game Settings")]
        [Tooltip("Whether to initialize systems on start")]
        [SerializeField] private bool initializeOnStart = true;
        
        private bool isInitialized = false;
        
        /// <summary>
        /// Whether the game is initialized.
        /// </summary>
        public bool IsInitialized => isInitialized;
        
        private void Awake()
        {
            // Ensure EventManager exists
            if (eventManager == null)
            {
                eventManager = EventManager.Instance;
            }
            
            // Find system references if not assigned
            if (playerStats == null)
            {
                playerStats = FindObjectOfType<PlayerStats>();
            }
            
            if (tsunamiManager == null)
            {
                tsunamiManager = FindObjectOfType<TsunamiManager>();
            }
            
            if (buildingManager == null)
            {
                buildingManager = FindObjectOfType<BuildingManager>();
            }
        }
        
        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeGame();
            }
        }
        
        /// <summary>
        /// Initializes the game and all systems.
        /// </summary>
        public void InitializeGame()
        {
            if (isInitialized) return;
            
            // Initialize player stats
            if (playerStats != null)
            {
                playerStats.Initialize();
            }
            
            isInitialized = true;
            Debug.Log("GameManager: Game initialized.");
        }
        
        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void RestartGame()
        {
            // Reset player
            if (playerStats != null)
            {
                if (playerStats.Health != null)
                {
                    playerStats.Health.Revive();
                }
                
                playerStats.Initialize();
            }
            
            // Reset tsunami manager (if needed)
            // Note: TsunamiManager handles its own state
            
            Debug.Log("GameManager: Game restarted.");
        }
        
        /// <summary>
        /// Quits the game.
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
