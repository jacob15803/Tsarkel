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
        
        /// <summary>
        /// Current state of the predator.
        /// </summary>
        public PredatorStateMachine.PredatorState CurrentState => stateMachine.CurrentState;
        
        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            detection = GetComponent<PredatorDetection>();
            
            if (detection == null)
            {
                detection = gameObject.AddComponent<PredatorDetection>();
            }
            
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
        /// </summary>
        private void UpdateAttack()
        {
            if (navAgent == null || detection == null || predatorData == null) return;
            
            navAgent.speed = 0f; // Stop to attack
            
            // Face player
            Vector3 directionToPlayer = detection.GetDirectionToPlayer();
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
            }
            
            // Attack if cooldown is ready
            if (Time.time - lastAttackTime >= predatorData.AttackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
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
            // Return to pool or destroy
            Destroy(gameObject);
        }
    }
}
