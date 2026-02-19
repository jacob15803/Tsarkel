using System.Collections.Generic;
using UnityEngine;
using Tsarkel.ScriptableObjects.Tribal;

namespace Tsarkel.Systems.Tribal
{
    /// <summary>
    /// Handles tribal ambush spawning and management.
    /// Spawns ambushes based on hostility level and cooldowns.
    /// </summary>
    public class TribalAmbush : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tribal configuration asset")]
        [SerializeField] private TribalConfig config;
        
        [Header("Ambush Settings")]
        [Tooltip("Ambush spawn point prefab or locations")]
        [SerializeField] private Transform[] ambushSpawnPoints;
        
        [Tooltip("Ambush NPC prefab")]
        [SerializeField] private GameObject ambushNPCPrefab;
        
        [Header("Dependencies")]
        [Tooltip("Tribal hostility meter reference")]
        [SerializeField] private TribalHostilityMeter hostilityMeter;
        
        private float lastAmbushTime = 0f;
        
        private void Start()
        {
            if (hostilityMeter == null)
            {
                hostilityMeter = FindObjectOfType<TribalHostilityMeter>();
            }
        }
        
        /// <summary>
        /// Checks if an ambush should be triggered.
        /// </summary>
        public bool ShouldTriggerAmbush()
        {
            if (config == null || hostilityMeter == null) return false;
            
            var stage = hostilityMeter.CurrentStage;
            
            // Only trigger in warning, escalation, or hostile stages
            if (stage == TribalHostilityMeter.HostilityStage.Neutral) return false;
            
            // Check cooldown
            float cooldown = GetAmbushCooldownForStage(stage);
            if (Time.time - lastAmbushTime < cooldown) return false;
            
            // Random chance based on stage
            float chance = GetAmbushChanceForStage(stage);
            return Random.value < chance;
        }
        
        /// <summary>
        /// Triggers an ambush.
        /// </summary>
        public void TriggerAmbush()
        {
            if (config == null || hostilityMeter == null || ambushNPCPrefab == null) return;
            
            var stage = hostilityMeter.CurrentStage;
            int ambushCount = GetAmbushCountForStage(stage);
            
            // Spawn ambush NPCs
            for (int i = 0; i < ambushCount; i++)
            {
                SpawnAmbushNPC();
            }
            
            lastAmbushTime = Time.time;
            EventManager.Instance.InvokeTribalAmbushTriggered();
        }
        
        /// <summary>
        /// Spawns a single ambush NPC.
        /// </summary>
        private void SpawnAmbushNPC()
        {
            if (ambushNPCPrefab == null) return;
            
            Vector3 spawnPosition = GetRandomAmbushSpawnPosition();
            if (spawnPosition == Vector3.zero) return;
            
            GameObject npc = Instantiate(ambushNPCPrefab, spawnPosition, Quaternion.identity);
            // NPC would have its own AI/behavior component
        }
        
        /// <summary>
        /// Gets a random ambush spawn position.
        /// </summary>
        private Vector3 GetRandomAmbushSpawnPosition()
        {
            if (ambushSpawnPoints == null || ambushSpawnPoints.Length == 0)
            {
                // Fallback: spawn near player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector2 randomCircle = Random.insideUnitCircle.normalized * 10f;
                    return player.transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
                }
                return Vector3.zero;
            }
            
            Transform spawnPoint = ambushSpawnPoints[Random.Range(0, ambushSpawnPoints.Length)];
            return spawnPoint.position;
        }
        
        /// <summary>
        /// Gets ambush cooldown for the current stage.
        /// </summary>
        private float GetAmbushCooldownForStage(TribalHostilityMeter.HostilityStage stage)
        {
            if (config == null) return 300f;
            
            switch (stage)
            {
                case TribalHostilityMeter.HostilityStage.Warning:
                    return config.BaseAmbushCooldown;
                case TribalHostilityMeter.HostilityStage.Escalation:
                    return config.EscalationAmbushCooldown;
                case TribalHostilityMeter.HostilityStage.Hostile:
                    return config.HostileAmbushCooldown;
                default:
                    return config.BaseAmbushCooldown;
            }
        }
        
        /// <summary>
        /// Gets ambush count for the current stage.
        /// </summary>
        private int GetAmbushCountForStage(TribalHostilityMeter.HostilityStage stage)
        {
            if (config == null) return 1;
            
            switch (stage)
            {
                case TribalHostilityMeter.HostilityStage.Warning:
                    return config.WarningAmbushCount;
                case TribalHostilityMeter.HostilityStage.Escalation:
                    return config.EscalationAmbushCount;
                case TribalHostilityMeter.HostilityStage.Hostile:
                    return config.HostileAmbushCount;
                default:
                    return 1;
            }
        }
        
        /// <summary>
        /// Gets ambush chance for the current stage.
        /// </summary>
        private float GetAmbushChanceForStage(TribalHostilityMeter.HostilityStage stage)
        {
            switch (stage)
            {
                case TribalHostilityMeter.HostilityStage.Warning:
                    return 0.1f; // 10% chance
                case TribalHostilityMeter.HostilityStage.Escalation:
                    return 0.3f; // 30% chance
                case TribalHostilityMeter.HostilityStage.Hostile:
                    return 0.5f; // 50% chance
                default:
                    return 0f;
            }
        }
    }
}
