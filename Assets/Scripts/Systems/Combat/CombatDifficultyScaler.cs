using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.Combat;

namespace Tsarkel.Systems.Combat
{
    /// <summary>
    /// Tracks in-game days and exposes scaled combat difficulty values.
    /// Bridges TimeManager with CombatConfig's difficulty scaling methods.
    ///
    /// Other systems (EnemyAttackTelegraph, EnemyAttackPattern) query this
    /// component to get the current telegraph duration and reaction window
    /// appropriate for the player's progression.
    ///
    /// Singleton — place one instance in the scene alongside CombatManager.
    /// </summary>
    public class CombatDifficultyScaler : MonoBehaviour
    {
        // ─── Singleton ────────────────────────────────────────────────────────
        private static CombatDifficultyScaler _instance;

        /// <summary>Global singleton access.</summary>
        public static CombatDifficultyScaler Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CombatDifficultyScaler");
                    _instance = go.AddComponent<CombatDifficultyScaler>();
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
        [Tooltip("TimeManager reference for tracking in-game days")]
        [SerializeField] private TimeManager timeManager;

        // ─── State ────────────────────────────────────────────────────────────
        private float daysPassed = 0f;
        private float lastTimeOfDay = 0f;

        /// <summary>Total in-game days elapsed since the game started.</summary>
        public float DaysPassed => daysPassed;

        /// <summary>
        /// Current scaled telegraph duration based on days passed.
        /// Decreases as the game progresses (harder = shorter warning).
        /// </summary>
        public float CurrentTelegraphDuration =>
            config != null ? config.GetScaledTelegraphDuration(daysPassed) : 1.0f;

        /// <summary>
        /// Current scaled reaction window based on days passed.
        /// Decreases as the game progresses (harder = less time to react).
        /// </summary>
        public float CurrentReactionWindow =>
            config != null ? config.GetScaledReactionWindow(daysPassed) : 0.5f;

        /// <summary>
        /// Current difficulty tier: 0 = Early, 1 = Mid, 2 = Late.
        /// </summary>
        public int DifficultyTier
        {
            get
            {
                if (config == null) return 0;
                if (daysPassed >= config.LateGameDay)  return 2;
                if (daysPassed >= config.MidGameDay)   return 1;
                return 0;
            }
        }

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
            if (timeManager == null)
                timeManager = FindObjectOfType<TimeManager>();

            if (timeManager != null)
                lastTimeOfDay = timeManager.TimeOfDay;
        }

        private void Update()
        {
            TrackDays();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>
        /// Manually sets the days passed (e.g., when loading a save).
        /// </summary>
        public void SetDaysPassed(float days)
        {
            daysPassed = Mathf.Max(0f, days);
        }

        /// <summary>
        /// Returns whether combo attacks are unlocked at the current difficulty.
        /// Combos unlock at mid-game tier.
        /// </summary>
        public bool AreComboAttacksUnlocked => DifficultyTier >= 1;

        /// <summary>
        /// Returns whether fake telegraphs are unlocked at the current difficulty.
        /// Fake telegraphs unlock at late-game tier.
        /// </summary>
        public bool AreFakeTelegraphsUnlocked => DifficultyTier >= 2;

        // ─── Private Helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Detects midnight crossings in TimeManager.TimeOfDay (0–1 range)
        /// and increments daysPassed.
        /// </summary>
        private void TrackDays()
        {
            if (timeManager == null) return;

            float currentTime = timeManager.TimeOfDay;

            // Midnight crossing: current time wrapped back below last recorded time
            if (currentTime < lastTimeOfDay)
                daysPassed += 1f;

            lastTimeOfDay = currentTime;
        }
    }
}
