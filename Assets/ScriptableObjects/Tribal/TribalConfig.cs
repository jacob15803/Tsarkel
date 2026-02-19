using UnityEngine;

namespace Tsarkel.ScriptableObjects.Tribal
{
    /// <summary>
    /// Configuration data for tribal presence system.
    /// Defines hostility mechanics and ambush behavior.
    /// </summary>
    [CreateAssetMenu(fileName = "TribalConfig", menuName = "Tsarkel/Tribal/Tribal Config")]
    public class TribalConfig : ScriptableObject
    {
        [Header("Hostility Settings")]
        [Tooltip("Maximum hostility level")]
        [SerializeField] private float maxHostility = 100f;
        
        [Tooltip("Hostility increase per second while in territory")]
        [SerializeField] private float hostilityIncreasePerSecond = 0.1f;
        
        [Tooltip("Hostility decrease per second when outside territory")]
        [SerializeField] private float hostilityDecreasePerSecond = 0.2f;
        
        [Header("Hostility Stages")]
        [Tooltip("Hostility level for warning stage (0-40)")]
        [SerializeField] private float warningStageThreshold = 40f;
        
        [Tooltip("Hostility level for escalation stage (40-70)")]
        [SerializeField] private float escalationStageThreshold = 70f;
        
        [Tooltip("Hostility level for hostile stage (70-100)")]
        [SerializeField] private float hostileStageThreshold = 70f;
        
        [Header("Action-Based Hostility")]
        [Tooltip("Hostility increase for building in territory")]
        [SerializeField] private float buildingHostilityIncrease = 5f;
        
        [Tooltip("Hostility increase for resource gathering")]
        [SerializeField] private float resourceGatheringHostilityIncrease = 2f;
        
        [Tooltip("Hostility increase for weapon crafting")]
        [SerializeField] private float weaponCraftingHostilityIncrease = 3f;
        
        [Tooltip("Hostility increase for attacking wildlife near territory")]
        [SerializeField] private float aggressiveActionHostilityIncrease = 4f;
        
        [Header("Ambush Settings")]
        [Tooltip("Base ambush cooldown (seconds)")]
        [SerializeField] private float baseAmbushCooldown = 300f; // 5 minutes
        
        [Tooltip("Ambush cooldown at escalation stage (seconds)")]
        [SerializeField] private float escalationAmbushCooldown = 180f; // 3 minutes
        
        [Tooltip("Ambush cooldown at hostile stage (seconds)")]
        [SerializeField] private float hostileAmbushCooldown = 60f; // 1 minute
        
        [Tooltip("Ambush spawn count at warning stage")]
        [SerializeField] private int warningAmbushCount = 1;
        
        [Tooltip("Ambush spawn count at escalation stage")]
        [SerializeField] private int escalationAmbushCount = 2;
        
        [Tooltip("Ambush spawn count at hostile stage")]
        [SerializeField] private int hostileAmbushCount = 3;

        // Public properties
        public float MaxHostility => maxHostility;
        public float HostilityIncreasePerSecond => hostilityIncreasePerSecond;
        public float HostilityDecreasePerSecond => hostilityDecreasePerSecond;
        public float WarningStageThreshold => warningStageThreshold;
        public float EscalationStageThreshold => escalationStageThreshold;
        public float HostileStageThreshold => hostileStageThreshold;
        public float BuildingHostilityIncrease => buildingHostilityIncrease;
        public float ResourceGatheringHostilityIncrease => resourceGatheringHostilityIncrease;
        public float WeaponCraftingHostilityIncrease => weaponCraftingHostilityIncrease;
        public float AggressiveActionHostilityIncrease => aggressiveActionHostilityIncrease;
        public float BaseAmbushCooldown => baseAmbushCooldown;
        public float EscalationAmbushCooldown => escalationAmbushCooldown;
        public float HostileAmbushCooldown => hostileAmbushCooldown;
        public int WarningAmbushCount => warningAmbushCount;
        public int EscalationAmbushCount => escalationAmbushCount;
        public int HostileAmbushCount => hostileAmbushCount;
    }
}
