using UnityEngine;

namespace Tsarkel.AI.Wildlife
{
    /// <summary>
    /// State machine for predator AI behavior.
    /// Manages state transitions and state-specific logic.
    /// </summary>
    public class PredatorStateMachine
    {
        public enum PredatorState
        {
            Patrol,
            Investigate,
            Chase,
            Attack,
            Return
        }
        
        private PredatorState currentState = PredatorState.Patrol;
        private float stateTimer = 0f;
        
        /// <summary>
        /// Current state.
        /// </summary>
        public PredatorState CurrentState => currentState;
        
        /// <summary>
        /// Time spent in current state.
        /// </summary>
        public float StateTimer => stateTimer;
        
        /// <summary>
        /// Changes to a new state.
        /// </summary>
        public void ChangeState(PredatorState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                stateTimer = 0f;
            }
        }
        
        /// <summary>
        /// Updates the state timer.
        /// </summary>
        public void UpdateTimer(float deltaTime)
        {
            stateTimer += deltaTime;
        }
        
        /// <summary>
        /// Resets the state timer.
        /// </summary>
        public void ResetTimer()
        {
            stateTimer = 0f;
        }
    }
}
