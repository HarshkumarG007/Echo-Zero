using System;

namespace EchoZero.AI.Utility
{
    /// <summary>
    /// Base class for scoring an action based on some environment input.
    /// Subclasses should read from a context object, not capture outer fields via closures.
    /// </summary>
    public abstract class UtilityScorer
    {
        /// <summary>
        /// An optional response curve (identity curve by default).
        /// </summary>
        public Func<float, float> Curve { get; set; }

        protected UtilityScorer(Func<float, float> curve = null)
        {
            // Default to linear y = x
            Curve = curve ?? (x => x);
        }

        public abstract float Score();
    }

    /// <summary>
    /// Lightweight scorer created from a delegate — useful for tests and simple cases.
    /// For complex in-game use, prefer typed subclasses that take an explicit context.
    /// </summary>
    public sealed class DelegateScorer : UtilityScorer
    {
        private readonly Func<float> _evaluator;

        public DelegateScorer(Func<float> evaluator, Func<float, float> curve = null) : base(curve)
        {
            _evaluator = evaluator;
        }

        public override float Score()
        {
            float raw = _evaluator();
            return Curve(raw);
        }
    }
}
