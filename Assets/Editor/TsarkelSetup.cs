#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Linq;
using Tsarkel.Managers;
using Tsarkel.Player;
using Tsarkel.Systems.Survival;
using Tsarkel.Systems.Tsunami;
using Tsarkel.Systems.Building;
using Tsarkel.Systems.Zone;
using Tsarkel.UI;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Editor
{
    /// <summary>
    /// Professional Unity Editor automation tool for one-click project bootstrap.
    /// Creates all required ScriptableObjects, GameObjects, components, and cross-references.
    /// </summary>
    public static class TsarkelSetup
    {
        // Constants
        private const string CONFIG_PATH = "Assets/ScriptableObjects/Config";
        
        // Cached references for cross-assignment
        private static GameObject playerObj;
        private static GameObject gameManagerObj;
        private static GameObject eventManagerObj;
        private static GameObject zoneManagerObj;
        private static GameObject tsunamiManagerObj;
        private static GameObject buildingManagerObj;
        private static GameObject waterControllerObj;
        private static GameObject safeElevationDetectorObj;
        private static GameObject tsunamiIntensityScalerObj;
        private static GameObject canvasObj;
        
        // Config references
        private static PlayerConfig playerConfig;
        private static TsunamiConfig tsunamiConfig;
        private static SurvivalConfig survivalConfig;
        
        #region Menu Items
        
        /// <summary>
        /// Main menu item: Tsarkel → Setup Project
        /// Performs complete one-click project bootstrap.
        /// </summary>
        [MenuItem("Tsarkel/Setup Project")]
        public static void SetupProject()
        {
            try
            {
                Debug.Log("[TsarkelSetup] Starting project setup...");
                
                // Ensure we're in a scene
                if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
                {
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
                    Debug.Log("[TsarkelSetup] Created new scene");
                }
                
                // Create configs first
                CreateConfigs();
                
                // Create core scene
                CreateCoreScene();
                
                // Setup player
                SetupPlayer();
                
                // Setup systems
                SetupTsunamiSystem();
                SetupBuildingSystem();
                SetupUISystem();
                
                // Assign all configs
                AssignAllConfigs();
                
                // Assign cross-references
                AssignGameManagerReferences();
                AssignPlayerCrossReferences();
                AssignTsunamiCrossReferences();
                
                // Mark scene dirty and save
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorSceneManager.SaveOpenScenes();
                
                Debug.Log("[TsarkelSetup] Project setup complete! Press Play to test.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TsarkelSetup] Setup failed: {e.Message}\n{e.StackTrace}");
            }
        }
        
        /// <summary>
        /// Validation menu item: Tsarkel → Validate Project Setup
        /// Checks for missing configs, GameObjects, components, and references.
        /// </summary>
        [MenuItem("Tsarkel/Validate Project Setup")]
        public static void ValidateProjectSetup()
        {
            int issueCount = 0;
            
            Debug.Log("[TsarkelSetup] Starting validation...");
            
            // Validate configs
            issueCount += ValidateConfigs();
            
            // Validate GameObjects
            issueCount += ValidateGameObjects();
            
            // Validate components
            issueCount += ValidateComponents();
            
            // Validate references
            issueCount += ValidateReferences();
            
            if (issueCount == 0)
            {
                Debug.Log("[TsarkelSetup] Validation complete: No issues found! ✓");
            }
            else
            {
                Debug.LogWarning($"[TsarkelSetup] Validation complete: {issueCount} issues found. Please review warnings above.");
            }
        }
        
        #endregion
        
        #region Setup Methods
        
        /// <summary>
        /// Creates all required ScriptableObject configs if they don't exist.
        /// </summary>
        private static void CreateConfigs()
        {
            Debug.Log("[TsarkelSetup] Creating ScriptableObject configs...");
            
            // Ensure directory exists
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
                AssetDatabase.Refresh();
            }
            
            // Create PlayerConfig
            string playerConfigPath = $"{CONFIG_PATH}/PlayerConfig.asset";
            playerConfig = AssetDatabase.LoadAssetAtPath<PlayerConfig>(playerConfigPath);
            if (playerConfig == null)
            {
                playerConfig = ScriptableObject.CreateInstance<PlayerConfig>();
                AssetDatabase.CreateAsset(playerConfig, playerConfigPath);
                Debug.Log($"[TsarkelSetup] Created PlayerConfig at {playerConfigPath}");
            }
            else
            {
                Debug.Log($"[TsarkelSetup] PlayerConfig already exists at {playerConfigPath}");
            }
            
            // Create TsunamiConfig
            string tsunamiConfigPath = $"{CONFIG_PATH}/TsunamiConfig.asset";
            tsunamiConfig = AssetDatabase.LoadAssetAtPath<TsunamiConfig>(tsunamiConfigPath);
            if (tsunamiConfig == null)
            {
                tsunamiConfig = ScriptableObject.CreateInstance<TsunamiConfig>();
                AssetDatabase.CreateAsset(tsunamiConfig, tsunamiConfigPath);
                Debug.Log($"[TsarkelSetup] Created TsunamiConfig at {tsunamiConfigPath}");
            }
            else
            {
                Debug.Log($"[TsarkelSetup] TsunamiConfig already exists at {tsunamiConfigPath}");
            }
            
            // Create SurvivalConfig
            string survivalConfigPath = $"{CONFIG_PATH}/SurvivalConfig.asset";
            survivalConfig = AssetDatabase.LoadAssetAtPath<SurvivalConfig>(survivalConfigPath);
            if (survivalConfig == null)
            {
                survivalConfig = ScriptableObject.CreateInstance<SurvivalConfig>();
                AssetDatabase.CreateAsset(survivalConfig, survivalConfigPath);
                Debug.Log($"[TsarkelSetup] Created SurvivalConfig at {survivalConfigPath}");
            }
            else
            {
                Debug.Log($"[TsarkelSetup] SurvivalConfig already exists at {survivalConfigPath}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Creates core scene GameObjects: Ground, GameManager, EventManager, ZoneManager.
        /// </summary>
        private static void CreateCoreScene()
        {
            Debug.Log("[TsarkelSetup] Creating core scene objects...");
            
            // Create Ground (Plane)
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.position = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
                
                // Try to set tag if it exists
                try
                {
                    ground.tag = "Ground";
                }
                catch
                {
                    // Tag doesn't exist, that's okay
                }
                
                Debug.Log("[TsarkelSetup] Created Ground");
            }
            
            // Create GameManager
            gameManagerObj = FindOrCreateGameObject("GameManager", Vector3.zero);
            GetOrAddComponent<GameManager>(gameManagerObj);
            
            // Create EventManager
            eventManagerObj = FindOrCreateGameObject("EventManager", Vector3.zero);
            GetOrAddComponent<EventManager>(eventManagerObj);
            
            // Create ZoneManager
            zoneManagerObj = FindOrCreateGameObject("ZoneManager", Vector3.zero);
            GetOrAddComponent<ZoneManager>(zoneManagerObj);
        }
        
        /// <summary>
        /// Sets up the Player GameObject with all required components and PlayerCamera child.
        /// </summary>
        private static void SetupPlayer()
        {
            Debug.Log("[TsarkelSetup] Setting up Player...");
            
            // Create or find Player
            playerObj = FindOrCreateGameObject("Player", new Vector3(0, 1, 0));
            
            // Add CharacterController
            CharacterController charController = GetOrAddComponent<CharacterController>(playerObj);
            if (charController != null)
            {
                // Set reasonable defaults
                charController.height = 2f;
                charController.radius = 0.5f;
                charController.center = new Vector3(0, 1, 0);
            }
            
            // Add Player components
            GetOrAddComponent<PlayerController>(playerObj);
            GetOrAddComponent<PlayerStats>(playerObj);
            
            // Add Survival systems
            GetOrAddComponent<HealthSystem>(playerObj);
            GetOrAddComponent<StaminaSystem>(playerObj);
            GetOrAddComponent<HungerSystem>(playerObj);
            GetOrAddComponent<HydrationSystem>(playerObj);
            
            // Create PlayerCamera child
            Transform cameraTransform = playerObj.transform.Find("PlayerCamera");
            GameObject cameraObj;
            if (cameraTransform == null)
            {
                cameraObj = new GameObject("PlayerCamera");
                cameraObj.transform.SetParent(playerObj.transform);
                cameraObj.transform.localPosition = new Vector3(0, 1.6f, 0);
                cameraObj.transform.localRotation = Quaternion.identity;
                Undo.RegisterCreatedObjectUndo(cameraObj, "Create PlayerCamera");
            }
            else
            {
                cameraObj = cameraTransform.gameObject;
            }
            
            // Add Camera component
            Camera cam = GetOrAddComponent<Camera>(cameraObj);
            if (cam != null)
            {
                cam.fieldOfView = 75f;
                // Set as main camera
                cameraObj.tag = "MainCamera";
            }
            
            // Add PlayerCamera component
            GetOrAddComponent<PlayerCamera>(cameraObj);
            
            Debug.Log("[TsarkelSetup] Player setup complete");
        }
        
        /// <summary>
        /// Sets up the Tsunami system GameObjects and components.
        /// </summary>
        private static void SetupTsunamiSystem()
        {
            Debug.Log("[TsarkelSetup] Setting up Tsunami system...");
            
            // Create TsunamiManager
            tsunamiManagerObj = FindOrCreateGameObject("TsunamiManager", Vector3.zero);
            GetOrAddComponent<TsunamiManager>(tsunamiManagerObj);
            
            // Create WaterController
            waterControllerObj = FindOrCreateGameObject("WaterController", Vector3.zero);
            GetOrAddComponent<WaterController>(waterControllerObj);
            
            // Create SafeElevationDetector
            safeElevationDetectorObj = FindOrCreateGameObject("SafeElevationDetector", Vector3.zero);
            GetOrAddComponent<SafeElevationDetector>(safeElevationDetectorObj);
            
            // Create TsunamiIntensityScaler
            tsunamiIntensityScalerObj = FindOrCreateGameObject("TsunamiIntensityScaler", Vector3.zero);
            GetOrAddComponent<TsunamiIntensityScaler>(tsunamiIntensityScalerObj);
            
            Debug.Log("[TsarkelSetup] Tsunami system setup complete");
        }
        
        /// <summary>
        /// Sets up the Building system GameObjects and components.
        /// </summary>
        private static void SetupBuildingSystem()
        {
            Debug.Log("[TsarkelSetup] Setting up Building system...");
            
            // Create BuildingManager
            buildingManagerObj = FindOrCreateGameObject("BuildingManager", Vector3.zero);
            GetOrAddComponent<BuildingManager>(buildingManagerObj);
            
            // Create BuildingPlacement
            GameObject buildingPlacementObj = FindOrCreateGameObject("BuildingPlacement", Vector3.zero);
            GetOrAddComponent<BuildingPlacement>(buildingPlacementObj);
            
            Debug.Log("[TsarkelSetup] Building system setup complete");
        }
        
        /// <summary>
        /// Sets up the UI system with Canvas and UIManager.
        /// </summary>
        private static void SetupUISystem()
        {
            Debug.Log("[TsarkelSetup] Setting up UI system...");
            
            // Find or create Canvas
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }
            else
            {
                canvasObj = canvas.gameObject;
            }
            
            // Add UIManager
            GetOrAddComponent<UIManager>(canvasObj);
            
            // Ensure EventSystem exists
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(eventSystemObj, "Create EventSystem");
            }
            
            // Create placeholder UI panels
            CreateUIPanel("HUD", canvasObj.transform);
            CreateUIPanel("BuildingMenu", canvasObj.transform);
            CreateUIPanel("PauseMenu", canvasObj.transform);
            CreateUIPanel("DeathScreen", canvasObj.transform);
            
            Debug.Log("[TsarkelSetup] UI system setup complete");
        }
        
        /// <summary>
        /// Creates a UI panel as a child of the specified parent.
        /// </summary>
        private static void CreateUIPanel(string name, Transform parent)
        {
            if (parent.Find(name) == null)
            {
                GameObject panel = new GameObject(name);
                panel.transform.SetParent(parent, false);
                RectTransform rectTransform = panel.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                panel.SetActive(false); // Hidden by default
                Undo.RegisterCreatedObjectUndo(panel, $"Create {name} Panel");
            }
        }
        
        #endregion
        
        #region Assignment Methods
        
        /// <summary>
        /// Assigns all ScriptableObject configs to their respective components.
        /// </summary>
        private static void AssignAllConfigs()
        {
            Debug.Log("[TsarkelSetup] Assigning configs to components...");
            
            if (playerConfig == null)
                playerConfig = LoadConfig<PlayerConfig>($"{CONFIG_PATH}/PlayerConfig.asset");
            if (tsunamiConfig == null)
                tsunamiConfig = LoadConfig<TsunamiConfig>($"{CONFIG_PATH}/TsunamiConfig.asset");
            if (survivalConfig == null)
                survivalConfig = LoadConfig<SurvivalConfig>($"{CONFIG_PATH}/SurvivalConfig.asset");
            
            if (playerObj != null)
            {
                // Assign PlayerConfig to PlayerController
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                if (playerController != null && playerConfig != null)
                {
                    AssignSerializedReference(playerController, "config", playerConfig);
                }
                
                // Assign PlayerConfig to PlayerCamera
                PlayerCamera playerCamera = playerObj.GetComponentInChildren<PlayerCamera>();
                if (playerCamera != null && playerConfig != null)
                {
                    AssignSerializedReference(playerCamera, "config", playerConfig);
                }
                
                // Assign SurvivalConfig to all survival systems
                HealthSystem healthSystem = playerObj.GetComponent<HealthSystem>();
                if (healthSystem != null && survivalConfig != null)
                {
                    AssignSerializedReference(healthSystem, "config", survivalConfig);
                }
                
                StaminaSystem staminaSystem = playerObj.GetComponent<StaminaSystem>();
                if (staminaSystem != null && survivalConfig != null)
                {
                    AssignSerializedReference(staminaSystem, "config", survivalConfig);
                }
                
                HungerSystem hungerSystem = playerObj.GetComponent<HungerSystem>();
                if (hungerSystem != null && survivalConfig != null)
                {
                    AssignSerializedReference(hungerSystem, "config", survivalConfig);
                }
                
                HydrationSystem hydrationSystem = playerObj.GetComponent<HydrationSystem>();
                if (hydrationSystem != null && survivalConfig != null)
                {
                    AssignSerializedReference(hydrationSystem, "config", survivalConfig);
                }
            }
            
            // Assign TsunamiConfig to TsunamiManager
            if (tsunamiManagerObj != null)
            {
                TsunamiManager tsunamiManager = tsunamiManagerObj.GetComponent<TsunamiManager>();
                if (tsunamiManager != null && tsunamiConfig != null)
                {
                    AssignSerializedReference(tsunamiManager, "config", tsunamiConfig);
                }
            }
            
            Debug.Log("[TsarkelSetup] Config assignment complete");
        }
        
        /// <summary>
        /// Assigns cross-references to GameManager component.
        /// </summary>
        private static void AssignGameManagerReferences()
        {
            Debug.Log("[TsarkelSetup] Assigning GameManager references...");
            
            if (gameManagerObj == null) return;
            
            GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
            if (gameManager == null) return;
            
            // Assign EventManager
            if (eventManagerObj != null)
            {
                EventManager eventManager = eventManagerObj.GetComponent<EventManager>();
                if (eventManager != null)
                {
                    AssignSerializedReference(gameManager, "eventManager", eventManager);
                }
            }
            
            // Assign PlayerStats
            if (playerObj != null)
            {
                PlayerStats playerStats = playerObj.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    AssignSerializedReference(gameManager, "playerStats", playerStats);
                }
            }
            
            // Assign TsunamiManager
            if (tsunamiManagerObj != null)
            {
                TsunamiManager tsunamiManager = tsunamiManagerObj.GetComponent<TsunamiManager>();
                if (tsunamiManager != null)
                {
                    AssignSerializedReference(gameManager, "tsunamiManager", tsunamiManager);
                }
            }
            
            // Assign BuildingManager
            if (buildingManagerObj != null)
            {
                BuildingManager buildingManager = buildingManagerObj.GetComponent<BuildingManager>();
                if (buildingManager != null)
                {
                    AssignSerializedReference(gameManager, "buildingManager", buildingManager);
                }
            }
        }
        
        /// <summary>
        /// Assigns cross-references between Player components.
        /// </summary>
        private static void AssignPlayerCrossReferences()
        {
            Debug.Log("[TsarkelSetup] Assigning Player cross-references...");
            
            if (playerObj == null) return;
            
            // Get all components
            PlayerStats playerStats = playerObj.GetComponent<PlayerStats>();
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            PlayerCamera playerCamera = playerObj.GetComponentInChildren<PlayerCamera>();
            HealthSystem healthSystem = playerObj.GetComponent<HealthSystem>();
            StaminaSystem staminaSystem = playerObj.GetComponent<StaminaSystem>();
            HungerSystem hungerSystem = playerObj.GetComponent<HungerSystem>();
            HydrationSystem hydrationSystem = playerObj.GetComponent<HydrationSystem>();
            
            // Assign PlayerStats references
            if (playerStats != null)
            {
                if (healthSystem != null)
                    AssignSerializedReference(playerStats, "healthSystem", healthSystem);
                if (staminaSystem != null)
                    AssignSerializedReference(playerStats, "staminaSystem", staminaSystem);
                if (hungerSystem != null)
                    AssignSerializedReference(playerStats, "hungerSystem", hungerSystem);
                if (hydrationSystem != null)
                    AssignSerializedReference(playerStats, "hydrationSystem", hydrationSystem);
            }
            
            // Assign PlayerController references
            if (playerController != null)
            {
                if (staminaSystem != null)
                    AssignSerializedReference(playerController, "staminaSystem", staminaSystem);
                
                if (safeElevationDetectorObj != null)
                {
                    SafeElevationDetector elevationDetector = safeElevationDetectorObj.GetComponent<SafeElevationDetector>();
                    if (elevationDetector != null)
                    {
                        AssignSerializedReference(playerController, "elevationDetector", elevationDetector);
                    }
                }
            }
            
            // Assign PlayerCamera references
            if (playerCamera != null)
            {
                AssignSerializedReference(playerCamera, "playerTransform", playerObj.transform);
            }
            
            // Assign HungerSystem and HydrationSystem references to HealthSystem
            if (hungerSystem != null && healthSystem != null)
            {
                AssignSerializedReference(hungerSystem, "healthSystem", healthSystem);
            }
            
            if (hydrationSystem != null && healthSystem != null)
            {
                AssignSerializedReference(hydrationSystem, "healthSystem", healthSystem);
            }
        }
        
        /// <summary>
        /// Assigns cross-references for Tsunami system components.
        /// </summary>
        private static void AssignTsunamiCrossReferences()
        {
            Debug.Log("[TsarkelSetup] Assigning Tsunami cross-references...");
            
            if (tsunamiManagerObj == null) return;
            
            TsunamiManager tsunamiManager = tsunamiManagerObj.GetComponent<TsunamiManager>();
            if (tsunamiManager == null) return;
            
            // Assign WaterController
            if (waterControllerObj != null)
            {
                WaterController waterController = waterControllerObj.GetComponent<WaterController>();
                if (waterController != null)
                {
                    AssignSerializedReference(tsunamiManager, "waterController", waterController);
                }
            }
            
            // Assign SafeElevationDetector
            if (safeElevationDetectorObj != null)
            {
                SafeElevationDetector elevationDetector = safeElevationDetectorObj.GetComponent<SafeElevationDetector>();
                if (elevationDetector != null)
                {
                    AssignSerializedReference(tsunamiManager, "elevationDetector", elevationDetector);
                }
            }
            
            // Assign PlayerController
            if (playerObj != null)
            {
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    AssignSerializedReference(tsunamiManager, "playerController", playerController);
                }
            }
            
            // Assign TsunamiIntensityScaler
            if (tsunamiIntensityScalerObj != null)
            {
                TsunamiIntensityScaler intensityScaler = tsunamiIntensityScalerObj.GetComponent<TsunamiIntensityScaler>();
                if (intensityScaler != null)
                {
                    AssignSerializedReference(tsunamiManager, "intensityScaler", intensityScaler);
                }
            }
        }
        
        #endregion
        
        #region Validation Methods
        
        /// <summary>
        /// Validates that all required ScriptableObject configs exist.
        /// </summary>
        private static int ValidateConfigs()
        {
            int issues = 0;
            
            string playerConfigPath = $"{CONFIG_PATH}/PlayerConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<PlayerConfig>(playerConfigPath) == null)
            {
                Debug.LogWarning($"[TsarkelSetup] Missing: PlayerConfig at {playerConfigPath}");
                issues++;
            }
            
            string tsunamiConfigPath = $"{CONFIG_PATH}/TsunamiConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<TsunamiConfig>(tsunamiConfigPath) == null)
            {
                Debug.LogWarning($"[TsarkelSetup] Missing: TsunamiConfig at {tsunamiConfigPath}");
                issues++;
            }
            
            string survivalConfigPath = $"{CONFIG_PATH}/SurvivalConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<SurvivalConfig>(survivalConfigPath) == null)
            {
                Debug.LogWarning($"[TsarkelSetup] Missing: SurvivalConfig at {survivalConfigPath}");
                issues++;
            }
            
            return issues;
        }
        
        /// <summary>
        /// Validates that all required GameObjects exist in the scene.
        /// </summary>
        private static int ValidateGameObjects()
        {
            int issues = 0;
            
            if (GameObject.Find("Ground") == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: Ground GameObject");
                issues++;
            }
            
            if (GameObject.Find("GameManager") == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: GameManager GameObject");
                issues++;
            }
            
            if (GameObject.Find("EventManager") == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: EventManager GameObject");
                issues++;
            }
            
            if (GameObject.Find("Player") == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: Player GameObject");
                issues++;
            }
            
            GameObject player = GameObject.Find("Player");
            if (player != null && player.transform.Find("PlayerCamera") == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: PlayerCamera child of Player");
                issues++;
            }
            
            return issues;
        }
        
        /// <summary>
        /// Validates that all required components exist on GameObjects.
        /// </summary>
        private static int ValidateComponents()
        {
            int issues = 0;
            
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null && gameManager.GetComponent<GameManager>() == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: GameManager component on GameManager");
                issues++;
            }
            
            GameObject eventManager = GameObject.Find("EventManager");
            if (eventManager != null && eventManager.GetComponent<EventManager>() == null)
            {
                Debug.LogWarning("[TsarkelSetup] Missing: EventManager component on EventManager");
                issues++;
            }
            
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                if (player.GetComponent<CharacterController>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: CharacterController on Player");
                    issues++;
                }
                if (player.GetComponent<PlayerController>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: PlayerController on Player");
                    issues++;
                }
                if (player.GetComponent<PlayerStats>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: PlayerStats on Player");
                    issues++;
                }
                if (player.GetComponent<HealthSystem>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: HealthSystem on Player");
                    issues++;
                }
                if (player.GetComponent<StaminaSystem>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: StaminaSystem on Player");
                    issues++;
                }
                if (player.GetComponent<HungerSystem>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: HungerSystem on Player");
                    issues++;
                }
                if (player.GetComponent<HydrationSystem>() == null)
                {
                    Debug.LogWarning("[TsarkelSetup] Missing: HydrationSystem on Player");
                    issues++;
                }
                
                Transform cameraTransform = player.transform.Find("PlayerCamera");
                if (cameraTransform != null)
                {
                    GameObject cameraObj = cameraTransform.gameObject;
                    if (cameraObj.GetComponent<Camera>() == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Missing: Camera component on PlayerCamera");
                        issues++;
                    }
                    if (cameraObj.GetComponent<PlayerCamera>() == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Missing: PlayerCamera component on PlayerCamera");
                        issues++;
                    }
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Validates that all component references are properly assigned.
        /// </summary>
        private static int ValidateReferences()
        {
            int issues = 0;
            
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                // Validate PlayerStats references
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    SerializedObject so = new SerializedObject(playerStats);
                    if (so.FindProperty("healthSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerStats.healthSystem");
                        issues++;
                    }
                    if (so.FindProperty("staminaSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerStats.staminaSystem");
                        issues++;
                    }
                    if (so.FindProperty("hungerSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerStats.hungerSystem");
                        issues++;
                    }
                    if (so.FindProperty("hydrationSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerStats.hydrationSystem");
                        issues++;
                    }
                }
                
                // Validate PlayerController references
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    SerializedObject so = new SerializedObject(playerController);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerController.config");
                        issues++;
                    }
                    if (so.FindProperty("staminaSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerController.staminaSystem");
                        issues++;
                    }
                }
                
                // Validate survival systems configs
                HealthSystem healthSystem = player.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    SerializedObject so = new SerializedObject(healthSystem);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: HealthSystem.config");
                        issues++;
                    }
                }
                
                HungerSystem hungerSystem = player.GetComponent<HungerSystem>();
                if (hungerSystem != null)
                {
                    SerializedObject so = new SerializedObject(hungerSystem);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: HungerSystem.config");
                        issues++;
                    }
                    if (so.FindProperty("healthSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: HungerSystem.healthSystem");
                        issues++;
                    }
                }
                
                HydrationSystem hydrationSystem = player.GetComponent<HydrationSystem>();
                if (hydrationSystem != null)
                {
                    SerializedObject so = new SerializedObject(hydrationSystem);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: HydrationSystem.config");
                        issues++;
                    }
                    if (so.FindProperty("healthSystem").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: HydrationSystem.healthSystem");
                        issues++;
                    }
                }
                
                // Validate PlayerCamera
                PlayerCamera playerCamera = player.GetComponentInChildren<PlayerCamera>();
                if (playerCamera != null)
                {
                    SerializedObject so = new SerializedObject(playerCamera);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerCamera.config");
                        issues++;
                    }
                    if (so.FindProperty("playerTransform").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: PlayerCamera.playerTransform");
                        issues++;
                    }
                }
            }
            
            // Validate GameManager references
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null)
            {
                GameManager gm = gameManager.GetComponent<GameManager>();
                if (gm != null)
                {
                    SerializedObject so = new SerializedObject(gm);
                    if (so.FindProperty("eventManager").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: GameManager.eventManager");
                        issues++;
                    }
                    if (so.FindProperty("playerStats").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: GameManager.playerStats");
                        issues++;
                    }
                }
            }
            
            // Validate TsunamiManager references (if exists)
            GameObject tsunamiManager = GameObject.Find("TsunamiManager");
            if (tsunamiManager != null)
            {
                TsunamiManager tm = tsunamiManager.GetComponent<TsunamiManager>();
                if (tm != null)
                {
                    SerializedObject so = new SerializedObject(tm);
                    if (so.FindProperty("config").objectReferenceValue == null)
                    {
                        Debug.LogWarning("[TsarkelSetup] Unassigned: TsunamiManager.config");
                        issues++;
                    }
                }
            }
            
            return issues;
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Gets an existing component or adds it if missing.
        /// </summary>
        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            if (obj == null) return null;
            
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
                Undo.RegisterCreatedObjectUndo(component, $"Add {typeof(T).Name}");
            }
            return component;
        }
        
        /// <summary>
        /// Assigns a reference to a SerializedProperty using SerializedObject.
        /// </summary>
        private static void AssignSerializedReference(Component target, string propertyName, UnityEngine.Object value)
        {
            if (target == null || value == null) return;
            
            SerializedObject serializedObj = new SerializedObject(target);
            SerializedProperty prop = serializedObj.FindProperty(propertyName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                serializedObj.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// Finds an existing GameObject or creates a new one.
        /// </summary>
        private static GameObject FindOrCreateGameObject(string name, Vector3 position)
        {
            GameObject obj = GameObject.Find(name);
            if (obj == null)
            {
                obj = new GameObject(name);
                obj.transform.position = position;
                Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            }
            return obj;
        }
        
        /// <summary>
        /// Loads a ScriptableObject config from the specified path.
        /// </summary>
        private static T LoadConfig<T>(string path) where T : ScriptableObject
        {
            T config = AssetDatabase.LoadAssetAtPath<T>(path);
            if (config == null)
            {
                Debug.LogWarning($"[TsarkelSetup] Config not found at {path}");
            }
            return config;
        }
        
        #endregion
    }
}
#endif
