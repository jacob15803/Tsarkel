using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Combat;

namespace Tsarkel.Systems.Combat
{
    /// <summary>
    /// Central coordinator for the directional combat system.
    /// Manages combat mode activation, time scale manipulation, and encounter tracking.
    /// Singleton — place one instance in the scene.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────
        private static CombatManager _instance;

        /// <summary>Global singleton access.</summary>
        public static CombatManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CombatManager");
                    _instance = go.AddComponent<CombatManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // ─── Inspector Fields ─────────────────────────────────────────────────
        [Header("Configuration")]
        [Tooltip("Combat configuration asset")]
        [SerializeField] private CombatConfig config;

        [Header("Dependencies")]
        [Tooltip("Player transform — used for distance checks")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("CombatStance component on the player")]
        [SerializeField] private CombatStance combatStance;

        // ─── State ────────────────────────────────────────────────────────────
        private CombatState currentState = CombatState.Idle;
        private GameObject activeEnemy;
        private float targetTimeScale = 1f;

        /// <summary>Current state of the combat encounter.</summary>
        public CombatState CurrentState => currentState;

        /// <summary>The enemy currently engaged in combat (null if none).</summary>
        public GameObject ActiveEnemy => activeEnemy;

        /// <summary>Whether combat mode is active or entering.</summary>
        public bool IsInCombat => currentState == CombatState.Active || currentState == CombatState.Entering;

        // ─── Unity Lifecycle ──────────────────────────────────────────────────
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
                return;
            }
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            if (combatStance == null && playerTransform != null)
            {
                combatStance = playerTransform.GetComponent<CombatStance>();
            }
        }

        private void Update()
        {
            UpdateTimeScale();
            CheckCombatExitCondition();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Enters combat mode against the specified enemy.
        /// Called by EnemyAttackTelegraph when an attack begins.
        /// </summary>
        public void EnterCombat(GameObject enemy)
        {
            if (enemy == null) return;

            // Already fighting this enemy — no-op
            if (activeEnemy == enemy && IsInCombat) return;

            activeEnemy = enemy;
            currentState = CombatState.Entering;
            targetTimeScale = config != null ? config.CombatTimeScale : 0.7f;

            // Notify player stance
            if (combatStance != null)
                combatStance.EnterCombatStance(enemy);

            EventManager.Instance.InvokeCombatStarted(enemy);
        }

        /// <summary>
        /// Exits combat mode and restores normal time scale.
        /// </summary>
        public void ExitCombat()
        {
            if (currentState == CombatState.Idle) return;

            currentState = CombatState.Exiting;
            targetTimeScale = 1f;

            if (combatStance != null)
                combatStance.ExitCombatStance();

            activeEnemy = null;
            EventManager.Instance.InvokeCombatEnded();
        }

        /// <summary>
        /// Called when the active enemy is defeated or despawned.
        /// </summary>
        public void OnEnemyDefeated(GameObject enemy)
        {
            if (enemy == activeEnemy)
                ExitCombat();
        }

        // ─── Private Helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Smoothly lerps Time.timeScale toward the target value.
        /// Uses unscaled delta time so the lerp is not affected by the slow-mo itself.
        /// </summary>
        private void UpdateTimeScale()
        {
            if (Mathf.Approximately(Time.timeScale, targetTimeScale)) return;

            float speed = config != null ? config.TimeScaleTransitionSpeed : 3f;
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, targetTimeScale, speed * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Keep physics in sync

            // Snap when close enough
            if (Mathf.Abs(Time.timeScale - targetTimeScale) < 0.01f)
            {
                Time.timeScale = targetTimeScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                // Finalise state transitions
                if (currentState == CombatState.Entering)
                    currentState = CombatState.Active;
                else if (currentState == CombatState.Exiting)
                    currentState = CombatState.Idle;
            }
        }

        /// <summary>
        /// Automatically exits combat if the enemy moves too far away.
        /// </summary>
        private void CheckCombatExitCondition()
        {
            if (!IsInCombat || activeEnemy == null || playerTransform == null) return;

            float exitDist = config != null ? config.CombatExitDistance : 20f;
            float dist = Vector3.Distance(playerTransform.position, activeEnemy.transform.position);

            if (dist > exitDist)
                ExitCombat();
        }

        private void OnDestroy()
        {
            // Restore time scale if this manager is destroyed mid-combat
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}
