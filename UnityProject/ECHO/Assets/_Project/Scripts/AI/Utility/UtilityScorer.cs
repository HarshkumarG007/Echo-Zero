using System;

namespace EchoZero.AI.Utility
{
    /// <summary>
    /// Base class for scoring an action based on some environment input.
    /// </summary>
    public abstract class UtilityScorer
    {
        // A simple curve evaluator (could be replaced by AnimationCurve in Unity)
        public Func<float, float> Curve { get; set; }

        protected UtilityScorer(Func<float, float> curve = null)
        {
            // Default to linear y = x
            Curve = curve ?? (x => x);
        }

        public abstract float Score();
    }
}
