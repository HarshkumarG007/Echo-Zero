using System.Collections.Generic;

namespace EchoZero.AI.Utility
{
    /// <summary>
    /// Base class for a single Utility Action (e.g. Patrol, Pursue).
    /// </summary>
    public abstract class UtilityAction
    {
        public string Name { get; private set; }
        private List<UtilityScorer> _scorers = new();

        protected UtilityAction(string name)
        {
            Name = name;
        }

        public void AddScorer(UtilityScorer scorer)
        {
            _scorers.Add(scorer);
        }

        public float ScoreAction()
        {
            if (_scorers.Count == 0) return 0f;

            float score = 1f;
            foreach (var scorer in _scorers)
            {
                float s = scorer.Score();
                
                // If any scorer returns 0 (veto), the whole action is vetoed.
                if (s <= 0f) return 0f;
                
                // Multiply scores (or average them depending on design, multiplying emphasizes vetoes and compounds probability)
                score *= s;
            }

            // Compensate for multiplication of multiple factors [0,1] pulling the average down
            // Modification: average instead of pure multiplication if we want softer curves, but multiplication with veto is standard.
            // Let's stick to multiplication for now, but ensure base score doesn't decay to 0 too fast.
            return score;
        }
    }
}
