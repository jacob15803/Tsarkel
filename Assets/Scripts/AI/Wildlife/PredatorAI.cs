using UnityEngine;
using UnityEngine.AI;
using Tsarkel.Managers;
using Tsarkel.ScriptableObjects.AI;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// Main predator AI controller.
    /// Handles navigation, state management, and combat.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PredatorAI : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Predator data asset")]
        [SerializeField] private PredatorData predatorData;
        
        [Header("Components")]
        [Tooltip("NavMesh agent")]
        [SerializeField] private NavMeshAgent navAgent;
        
        [Tooltip("Detection component")]
        [SerializeField] private PredatorDetection detection;
        
        [Header("Spawn Point")]
        [Tooltip("Spawn point to return to")]
        [SerializeField] private Transform spawnPoint;
        
        private PredatorStateMachine stateMachine;
        private float lastStateUpdate = 0f;
        private float lastAttackTime = 0f;
        private Vector3 patrolTarget;
        private float loseInterestTimer = 0f;
        private float currentHealth;

        // Combat system integration
        private EnemyAttackTelegraph attackTelegraph;
        private EnemyAttackPattern attackPattern;
        
        /// <summary>
        /// Current state of the predator.
        /// </summary>
        public PredatorStateMachine.PredatorState CurrentState => stateMachine.CurrentState;
        
        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            detection = GetComponent<PredatorDetection>();

            if (detection == null)
                detection = gameObject.AddComponent<PredatorDetection>();

            // Optional combat system components (added in Inspector or at runtime)
            attackTelegraph = GetComponent<EnemyAttackTelegraph>();
            attackPattern   = GetComponent<EnemyAttackPattern>();

            stateMachine = new PredatorStateMachine();
        }
        
        private void Start()
        {
            if (predatorData == null)
            {
                Debug.LogError($"PredatorAI on {gameObject.name}: PredatorData is not assigned!");
                return;
            }
            
            currentHealth = predatorData.Health;
            
            // Initialize NavMesh agent
            if (navAgent != null)
            {
                navAgent.speed = predatorData.PatrolSpeed;
            }
            
            // Set initial patrol target
            SetNewPatrolTarget();
        }
        
        private void Update()
        {
            if (predatorData == null) return;
            
            // Update state machine at intervals (optimization)
            if (Time.time - lastStateUpdate >= predatorData.StateUpdateInterval)
            {
                lastStateUpdate = Time.time;
                UpdateStateMachine();
            }
            
            // Update current state behavior
            UpdateCurrentState();
        }
        
        /// <summary>
        /// Updates the state machine logic.
        /// </summary>
        private void UpdateStateMachine()
        {
            if (detection == null || predatorData == null) return;
            
            bool canSeePlayer = detection.CanSeePlayer();
            bool canHearPlayer = detection.CanHearPlayer();
            float distanceToPlayer = detection.GetDistanceToPlayer();
            
            switch (stateMachine.CurrentState)
            {
                case PredatorStateMachine.PredatorState.Patrol:
                    if (canSeePlayer)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Chase);
                        EventManager.Instance.InvokePredatorDetectedPlayer(gameObject);
                    }
                    else if (canHearPlayer)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Investigate);
                    }
                    break;
                    
                case PredatorStateMachine.PredatorState.Investigate:
                    if (canSeePlayer)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Chase);
                        EventManager.Instance.InvokePredatorDetectedPlayer(gameObject);
                    }
                    else if (stateMachine.StateTimer > 5f) // Give up investigating after 5 seconds
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Patrol);
                    }
                    break;
                    
                case PredatorStateMachine.PredatorState.Chase:
                    if (distanceToPlayer <= predatorData.AttackRange)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Attack);
                        // Notify CombatManager to enter combat mode
                        Systems.Combat.CombatManager.Instance.EnterCombat(gameObject);
                    }
                    else if (!canSeePlayer && !canHearPlayer)
                    {
                        loseInterestTimer += predatorData.StateUpdateInterval;
                        if (loseInterestTimer >= predatorData.LoseInterestTime)
                        {
                            stateMachine.ChangeState(PredatorStateMachine.PredatorState.Return);
                            loseInterestTimer = 0f;
                        }
                    }
                    else
                    {
                        loseInterestTimer = 0f;
                    }
                    break;
                    
                case PredatorStateMachine.PredatorState.Attack:
                    if (distanceToPlayer > predatorData.AttackRange * 1.5f)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Chase);
                    }
                    break;
                    
                case PredatorStateMachine.PredatorState.Return:
                    if (Vector3.Distance(transform.position, spawnPoint != null ? spawnPoint.position : transform.position) < 2f)
                    {
                        stateMachine.ChangeState(PredatorStateMachine.PredatorState.Patrol);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Updates behavior for the current state.
        /// </summary>
        private void UpdateCurrentState()
        {
            if (navAgent == null || predatorData == null) return;
            
            switch (stateMachine.CurrentState)
            {
                case PredatorStateMachine.PredatorState.Patrol:
                    UpdatePatrol();
                    break;
                    
                case PredatorStateMachine.PredatorState.Investigate:
                    UpdateInvestigate();
                    break;
                    
                case PredatorStateMachine.PredatorState.Chase:
                    UpdateChase();
                    break;
                    
                case PredatorStateMachine.PredatorState.Attack:
                    UpdateAttack();
                    break;
                    
                case PredatorStateMachine.PredatorState.Return:
                    UpdateReturn();
                    break;
            }
        }
        
        /// <summary>
        /// Updates patrol behavior.
        /// </summary>
        private void UpdatePatrol()
        {
            if (navAgent == null || predatorData == null) return;
            
            navAgent.speed = predatorData.PatrolSpeed;
            
            // Check if reached patrol target
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                SetNewPatrolTarget();
            }
        }
        
        /// <summary>
        /// Updates investigate behavior.
        /// </summary>
        private void UpdateInvestigate()
        {
            if (navAgent == null || detection == null) return;
            
            navAgent.speed = predatorData.PatrolSpeed;
            
            // Move toward last known player position or sound source
            Vector3 investigateTarget = detection.GetDirectionToPlayer() * predatorData.InvestigationRadius;
            navAgent.SetDestination(transform.position + investigateTarget);
        }
        
        /// <summary>
        /// Updates chase behavior.
        /// </summary>
        private void UpdateChase()
        {
            if (navAgent == null || detection == null || predatorData == null) return;
            
            navAgent.speed = predatorData.ChaseSpeed;
            
            // Chase player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                navAgent.SetDestination(player.transform.position);
            }
        }
        
        /// <summary>
        /// Updates attack behavior.
        /// Uses EnemyAttackTelegraph if available (directional combat system).
        /// Falls back to legacy direct damage if no telegraph component is present.
        /// </summary>
        private void UpdateAttack()
        {
            if (navAgent == null || detection == null || predatorData == null) return;

            navAgent.speed = 0f; // Stop to attack

            // Face player
            Vector3 directionToPlayer = detection.GetDirectionToPlayer();
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
            }

            // Attack if cooldown is ready
            if (Time.time - lastAttackTime >= predatorData.AttackCooldown)
            {
                lastAttackTime = Time.time;

                // ── Directional Combat System ─────────────────────────────────
                if (attackTelegraph != null && !attackTelegraph.IsExecutingPattern)
                {
                    // Update day count on pattern component if available
                    float days = 0f;
                    var timeManager = FindObjectOfType<Managers.TimeManager>();
                    if (attackPattern != null && timeManager != null)
                    {
                        attackPattern.CurrentDaysPassed = days; // TimeManager doesn't expose DaysPassed directly
                    }

                    attackTelegraph.ExecuteAttack(days);
                }
                else if (attackTelegraph == null)
                {
                    // ── Legacy Direct Attack (no telegraph component) ─────────
                    PerformAttack();
                }
                // If telegraph is executing, wait for it to finish before next attack
            }
        }
        
        /// <summary>
        /// Updates return behavior.
        /// </summary>
        private void UpdateReturn()
        {
            if (navAgent == null || spawnPoint == null) return;
            
            navAgent.speed = predatorData.PatrolSpeed;
            navAgent.SetDestination(spawnPoint.position);
        }
        
        /// <summary>
        /// Sets a new random patrol target.
        /// </summary>
        private void SetNewPatrolTarget()
        {
            if (navAgent == null || predatorData == null) return;
            
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Vector2 randomCircle = Random.insideUnitCircle * predatorData.PatrolRadius;
            patrolTarget = spawnPos + new Vector3(randomCircle.x, 0f, randomCircle.y);
            
            navAgent.SetDestination(patrolTarget);
        }
        
        /// <summary>
        /// Performs an attack on the player.
        /// </summary>
        private void PerformAttack()
        {
            if (detection == null || predatorData == null) return;
            
            float distance = detection.GetDistanceToPlayer();
            if (distance <= predatorData.AttackRange)
            {
                // Damage player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var playerStats = player.GetComponent<Player.PlayerStats>();
                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(predatorData.AttackDamage, predatorData.PredatorName);
                        EventManager.Instance.InvokePredatorAttackedPlayer(gameObject, predatorData.AttackDamage);
                    }
                }
            }
        }
        
        /// <summary>
        /// Sets the spawn point for this predator.
        /// </summary>
        public void SetSpawnPoint(Transform point)
        {
            spawnPoint = point;
        }

        /// <summary>
        /// Returns the base attack damage from PredatorData.
        /// Used by EnemyAttackTelegraph to determine damage before dodge modifiers.
        /// </summary>
        public float GetAttackDamage()
        {
            return predatorData != null ? predatorData.AttackDamage : 10f;
        }

        /// <summary>
        /// Applies a stun to this predator for the specified duration.
        /// Called by ParrySystem on a successful parry.
        /// </summary>
        public void ApplyStun(float duration)
        {
            if (duration <= 0f) return;
            StartCoroutine(StunCoroutine(duration));
        }

        private System.Collections.IEnumerator StunCoroutine(float duration)
        {
            // Disable state updates and stop navigation during stun
            bool wasEnabled = enabled;
            enabled = false;
            if (navAgent != null) navAgent.isStopped = true;

            yield return new WaitForSeconds(duration);

            enabled = wasEnabled;
            if (navAgent != null) navAgent.isStopped = false;
        }
        
        /// <summary>
        /// Applies damage to the predator.
        /// </summary>
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Handles predator death.
        /// </summary>
        private void Die()
        {
            // Notify CombatManager so combat mode exits cleanly
            Systems.Combat.CombatManager.Instance.OnEnemyDefeated(gameObject);

            // Return to pool or destroy
            Destroy(gameObject);
        }
    }
}
