namespace Tsarkel.Systems.Combat
{
    /// <summary>
    /// Outcome of a player dodge attempt against an incoming attack.
    /// </summary>
    public enum DodgeOutcome
    {
        /// <summary>Correct direction pressed within the reaction window — no damage, brief invincibility.</summary>
        Perfect,

        /// <summary>Side dodge (perpendicular direction) or slightly late — reduced damage.</summary>
        Partial,

        /// <summary>Wrong direction, opposite direction, or no input — full damage.</summary>
        Failed
    }

    /// <summary>
    /// Current state of the combat encounter.
    /// </summary>
    public enum CombatState
    {
        /// <summary>No active combat.</summary>
        Idle,

        /// <summary>Transitioning into combat mode (time scale lerping).</summary>
        Entering,

        /// <summary>Combat is fully active.</summary>
        Active,

        /// <summary>Transitioning out of combat mode.</summary>
        Exiting
    }
}
