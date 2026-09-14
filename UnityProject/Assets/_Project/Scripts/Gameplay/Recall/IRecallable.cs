namespace EchoZero.Gameplay.Recall
{
    /// <summary>
    /// Implemented by any world object that can be targeted by the RECALL ability.
    ///
    /// Three implementing types in the vertical slice:
    ///   - MemoryFragmentPickup  (Narrative layer)
    ///   - ReconstructionAnchor  (World layer)
    ///   - DriftController       (AI layer)
    ///
    /// RECALL is the single interaction verb. All player-world interaction flows
    /// through this interface.
    /// </summary>
    public interface IRecallable
    {
        /// <summary>
        /// Whether RECALL can currently be applied.
        /// False if already collected, already stabilised, or prerequisites not met.
        /// </summary>
        bool CanRecall { get; }

        /// <summary>
        /// Called by RecallSystem when the player successfully applies RECALL.
        /// Implementors are responsible for publishing their own domain events.
        /// </summary>
        void OnRecall();
    }
}
