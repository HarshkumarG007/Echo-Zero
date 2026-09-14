namespace EchoZero.AI.Drift
{
    /// <summary>
    /// States the Drift entity can be in.
    /// Evaluated by DriftUtilityBrain each tick.
    /// TASK: TASK-019 (split from deleted DriftStateMachine)
    /// </summary>
    public enum DriftState
    {
        Patrol,
        Alerted,
        Pursuing,
        Destabilizing,
        Stabilized
    }
}
