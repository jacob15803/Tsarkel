using UnityEngine;

namespace Tsarkel.ScriptableObjects.AI
{
    /// <summary>
    /// Configuration data for a predator AI.
    /// Defines behavior, stats, and detection parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "PredatorData", menuName = "Tsarkel/AI/Predator Data")]
    public class PredatorData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Predator name")]
        [SerializeField] private string predatorName = "Predator";
        
        [Header("Movement")]
        [Tooltip("Patrol speed")]
        [SerializeField] private float patrolSpeed = 2f;
        
        [Tooltip("Chase speed")]
        [SerializeField] private float chaseSpeed = 5f;
        
        [Tooltip("Attack range")]
        [SerializeField] private float attackRange = 2f;
        
        [Tooltip("Patrol radius from spawn point")]
        [SerializeField] private float patrolRadius = 10f;
        
        [Header("Detection")]
        [Tooltip("Sight detection radius")]
        [SerializeField] private float sightRadius = 15f;
        
        [Tooltip("Sight detection angle (degrees)")]
        [SerializeField] private float sightAngle = 90f;
        
        [Tooltip("Hearing detection radius")]
        [SerializeField] private float hearingRadius = 10f;
        
        [Tooltip("Investigation radius (how far to investigate sounds)")]
        [SerializeField] private float investigationRadius = 20f;
        
        [Header("Combat")]
        [Tooltip("Attack damage")]
        [SerializeField] private float attackDamage = 20f;
        
        [Tooltip("Attack cooldown (seconds)")]
        [SerializeField] private float attackCooldown = 2f;
        
        [Tooltip("Health points")]
        [SerializeField] private float health = 100f;
        
        [Header("Behavior")]
        [Tooltip("Time to lose interest and return to spawn (seconds)")]
        [SerializeField] private float loseInterestTime = 10f;
        
        [Tooltip("State update interval (seconds, for optimization)")]
        [SerializeField] private float stateUpdateInterval = 0.5f;

        // Public properties
        public string PredatorName => predatorName;
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float AttackRange => attackRange;
        public float PatrolRadius => patrolRadius;
        public float SightRadius => sightRadius;
        public float SightAngle => sightAngle;
        public float HearingRadius => hearingRadius;
        public float InvestigationRadius => investigationRadius;
        public float AttackDamage => attackDamage;
        public float AttackCooldown => attackCooldown;
        public float Health => health;
        public float LoseInterestTime => loseInterestTime;
        public float StateUpdateInterval => stateUpdateInterval;
    }
}
