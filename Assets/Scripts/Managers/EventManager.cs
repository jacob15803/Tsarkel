using System;
using UnityEngine;
using Tsarkel.ScriptableObjects.Zone;

namespace Tsarkel.Managers
{
    /// <summary>
    /// Centralized event manager for system communication.
    /// Uses C# events to decouple systems and enable event-driven architecture.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private static EventManager _instance;
        
        /// <summary>
        /// Singleton instance of EventManager.
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("EventManager");
                    _instance = go.AddComponent<EventManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        #region Player Events
        
        /// <summary>
        /// Event fired when player health changes.
        /// Parameters: (currentHealth, maxHealth)
        /// </summary>
        public event Action<float, float> OnPlayerHealthChanged;
        
        /// <summary>
        /// Event fired when player stamina changes.
        /// Parameters: (currentStamina, maxStamina)
        /// </summary>
        public event Action<float, float> OnPlayerStaminaChanged;
        
        /// <summary>
        /// Event fired when player hunger changes.
        /// Parameters: (currentHunger, maxHunger)
        /// </summary>
        public event Action<float, float> OnPlayerHungerChanged;
        
        /// <summary>
        /// Event fired when player hydration changes.
        /// Parameters: (currentHydration, maxHydration)
        /// </summary>
        public event Action<float, float> OnPlayerHydrationChanged;
        
        /// <summary>
        /// Event fired when player dies.
        /// </summary>
        public event Action OnPlayerDeath;
        
        /// <summary>
        /// Event fired when player takes damage.
        /// Parameters: (damageAmount, damageSource)
        /// </summary>
        public event Action<float, string> OnPlayerDamaged;
        
        #endregion
        
        #region Tsunami Events
        
        /// <summary>
        /// Event fired when tsunami warning phase begins.
        /// Parameters: (warningDuration)
        /// </summary>
        public event Action<float> OnTsunamiWarning;
        
        /// <summary>
        /// Event fired when tsunami wave phase begins.
        /// Parameters: (waveIntensity)
        /// </summary>
        public event Action<float> OnTsunamiWave;
        
        /// <summary>
        /// Event fired when tsunami wave ends.
        /// </summary>
        public event Action OnTsunamiWaveEnd;
        
        /// <summary>
        /// Event fired when water level changes.
        /// Parameters: (currentWaterLevel)
        /// </summary>
        public event Action<float> OnWaterLevelChanged;
        
        #endregion
        
        #region Building Events
        
        /// <summary>
        /// Event fired when a building is placed.
        /// Parameters: (buildingGameObject, buildingData)
        /// </summary>
        public event Action<GameObject, ScriptableObjects.Buildings.BuildingData> OnBuildingPlaced;
        
        /// <summary>
        /// Event fired when a building is destroyed.
        /// Parameters: (buildingGameObject)
        /// </summary>
        public event Action<GameObject> OnBuildingDestroyed;
        
        /// <summary>
        /// Event fired when a building takes damage.
        /// Parameters: (buildingGameObject, damageAmount, currentHealth, maxHealth)
        /// </summary>
        public event Action<GameObject, float, float, float> OnBuildingDamaged;
        
        /// <summary>
        /// Event fired when building placement mode is toggled.
        /// Parameters: (isInPlacementMode, buildingData)
        /// </summary>
        public event Action<bool, ScriptableObjects.Buildings.BuildingData> OnBuildingPlacementModeChanged;
        
        #endregion
        
        #region Game State Events
        
        /// <summary>
        /// Event fired when game is paused.
        /// </summary>
        public event Action OnGamePaused;
        
        /// <summary>
        /// Event fired when game is resumed.
        /// </summary>
        public event Action OnGameResumed;
        
        /// <summary>
        /// Event fired when game time changes (for day/night cycle if needed).
        /// Parameters: (timeOfDay)
        /// </summary>
        public event Action<float> OnGameTimeChanged;
        
        #endregion
        
        #region Zone Events
        
        /// <summary>
        /// Event fired when player enters a zone.
        /// Parameters: (zoneData)
        /// </summary>
        public event Action<ZoneData> OnZoneEntered;
        
        /// <summary>
        /// Event fired when player exits a zone.
        /// Parameters: (zoneData)
        /// </summary>
        public event Action<ZoneData> OnZoneExited;
        
        #endregion
        
        #region Wildlife Events
        
        /// <summary>
        /// Event fired when a predator is spawned.
        /// Parameters: (predatorGameObject)
        /// </summary>
        public event Action<GameObject> OnPredatorSpawned;
        
        /// <summary>
        /// Event fired when a predator detects the player.
        /// Parameters: (predatorGameObject)
        /// </summary>
        public event Action<GameObject> OnPredatorDetectedPlayer;
        
        /// <summary>
        /// Event fired when a predator attacks the player.
        /// Parameters: (predatorGameObject, damageAmount)
        /// </summary>
        public event Action<GameObject, float> OnPredatorAttackedPlayer;
        
        #endregion
        
        #region Tribal Events
        
        /// <summary>
        /// Event fired when tribal hostility level changes.
        /// Parameters: (hostilityLevel)
        /// </summary>
        public event Action<float> OnTribalHostilityChanged;
        
        /// <summary>
        /// Event fired when a tribal ambush is triggered.
        /// </summary>
        public event Action OnTribalAmbushTriggered;
        
        /// <summary>
        /// Event fired when player performs an action that affects tribal hostility.
        /// Parameters: (actionType, hostilityChange)
        /// </summary>
        public event Action<string, float> OnTribalActionPerformed;
        
        #endregion
        
        #region Crafting Events
        
        /// <summary>
        /// Event fired when an item is crafted.
        /// Parameters: (itemData, craftedGameObject)
        /// </summary>
        public event Action<ScriptableObjects.Items.ItemData, GameObject> OnItemCrafted;
        
        /// <summary>
        /// Event fired when weapon durability changes.
        /// Parameters: (weaponGameObject, currentDurability, maxDurability)
        /// </summary>
        public event Action<GameObject, float, float> OnWeaponDurabilityChanged;
        
        /// <summary>
        /// Event fired when a weapon breaks (durability reaches 0).
        /// Parameters: (weaponGameObject)
        /// </summary>
        public event Action<GameObject> OnWeaponBroke;
        
        #endregion
        
        #region Event Invocation Methods
        
        // Player Events
        public void InvokePlayerHealthChanged(float currentHealth, float maxHealth)
        {
            OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void InvokePlayerStaminaChanged(float currentStamina, float maxStamina)
        {
            OnPlayerStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        public void InvokePlayerHungerChanged(float currentHunger, float maxHunger)
        {
            OnPlayerHungerChanged?.Invoke(currentHunger, maxHunger);
        }
        
        public void InvokePlayerHydrationChanged(float currentHydration, float maxHydration)
        {
            OnPlayerHydrationChanged?.Invoke(currentHydration, maxHydration);
        }
        
        public void InvokePlayerDeath()
        {
            OnPlayerDeath?.Invoke();
        }
        
        public void InvokePlayerDamaged(float damageAmount, string damageSource = "Unknown")
        {
            OnPlayerDamaged?.Invoke(damageAmount, damageSource);
        }
        
        // Tsunami Events
        public void InvokeTsunamiWarning(float warningDuration)
        {
            OnTsunamiWarning?.Invoke(warningDuration);
        }
        
        public void InvokeTsunamiWave(float waveIntensity)
        {
            OnTsunamiWave?.Invoke(waveIntensity);
        }
        
        public void InvokeTsunamiWaveEnd()
        {
            OnTsunamiWaveEnd?.Invoke();
        }
        
        public void InvokeWaterLevelChanged(float currentWaterLevel)
        {
            OnWaterLevelChanged?.Invoke(currentWaterLevel);
        }
        
        // Building Events
        public void InvokeBuildingPlaced(GameObject building, ScriptableObjects.Buildings.BuildingData data)
        {
            OnBuildingPlaced?.Invoke(building, data);
        }
        
        public void InvokeBuildingDestroyed(GameObject building)
        {
            OnBuildingDestroyed?.Invoke(building);
        }
        
        public void InvokeBuildingDamaged(GameObject building, float damageAmount, float currentHealth, float maxHealth)
        {
            OnBuildingDamaged?.Invoke(building, damageAmount, currentHealth, maxHealth);
        }
        
        public void InvokeBuildingPlacementModeChanged(bool isInPlacementMode, ScriptableObjects.Buildings.BuildingData buildingData)
        {
            OnBuildingPlacementModeChanged?.Invoke(isInPlacementMode, buildingData);
        }
        
        // Game State Events
        public void InvokeGamePaused()
        {
            OnGamePaused?.Invoke();
        }
        
        public void InvokeGameResumed()
        {
            OnGameResumed?.Invoke();
        }
        
        public void InvokeGameTimeChanged(float timeOfDay)
        {
            OnGameTimeChanged?.Invoke(timeOfDay);
        }
        
        // Zone Events
        public void InvokeZoneEntered(ZoneData zoneData)
        {
            OnZoneEntered?.Invoke(zoneData);
        }
        
        public void InvokeZoneExited(ZoneData zoneData)
        {
            OnZoneExited?.Invoke(zoneData);
        }
        
        // Wildlife Events
        public void InvokePredatorSpawned(GameObject predator)
        {
            OnPredatorSpawned?.Invoke(predator);
        }
        
        public void InvokePredatorDetectedPlayer(GameObject predator)
        {
            OnPredatorDetectedPlayer?.Invoke(predator);
        }
        
        public void InvokePredatorAttackedPlayer(GameObject predator, float damageAmount)
        {
            OnPredatorAttackedPlayer?.Invoke(predator, damageAmount);
        }
        
        // Tribal Events
        public void InvokeTribalHostilityChanged(float hostilityLevel)
        {
            OnTribalHostilityChanged?.Invoke(hostilityLevel);
        }
        
        public void InvokeTribalAmbushTriggered()
        {
            OnTribalAmbushTriggered?.Invoke();
        }
        
        public void InvokeTribalActionPerformed(string actionType, float hostilityChange)
        {
            OnTribalActionPerformed?.Invoke(actionType, hostilityChange);
        }
        
        // Crafting Events
        public void InvokeItemCrafted(ScriptableObjects.Items.ItemData itemData, GameObject craftedObject)
        {
            OnItemCrafted?.Invoke(itemData, craftedObject);
        }
        
        public void InvokeWeaponDurabilityChanged(GameObject weapon, float currentDurability, float maxDurability)
        {
            OnWeaponDurabilityChanged?.Invoke(weapon, currentDurability, maxDurability);
        }
        
        public void InvokeWeaponBroke(GameObject weapon)
        {
            OnWeaponBroke?.Invoke(weapon);
        }
        
        #endregion
    }
}
