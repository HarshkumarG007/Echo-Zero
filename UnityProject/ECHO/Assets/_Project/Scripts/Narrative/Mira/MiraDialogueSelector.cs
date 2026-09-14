using UnityEngine;
using EchoZero.Core;
using EchoZero.Narrative.Fragments;

namespace EchoZero.Narrative.Mira
{
    /// <summary>
    /// Deterministically selects the appropriate dialogue line for Mira
    /// based on the current NarrativeState and FragmentRegistry state.
    /// All text is driven from <see cref="MiraDialogueConfig"/> (a ScriptableObject).
    /// TASK: TASK-008 / TASK-020
    /// </summary>
    public class MiraDialogueSelector
    {
        private readonly NarrativeState _narrativeState;
        private readonly FragmentRegistry _fragmentRegistry;
        private readonly MiraDialogueConfig _config;

        public MiraDialogueSelector(NarrativeState narrativeState, FragmentRegistry fragmentRegistry, MiraDialogueConfig config = null)
        {
            _narrativeState = narrativeState;
            _fragmentRegistry = fragmentRegistry;
            _config = config;
        }

        /// <summary>
        /// Returns the current dialogue line Mira should speak.
        /// </summary>
        public string GetCurrentDialogue()
        {
            if (_narrativeState.HasFlag(NarrativeFlags.RevealTriggered))
            {
                // Post-reveal, she is silent/gone.
                return string.Empty;
            }

            if (_fragmentRegistry != null && _fragmentRegistry.IsPlayerMustChooseState)
            {
                return Line(_config?.ContradictionLine, "Two memories. They cannot both be true. You must decide what happened.");
            }

            if (_narrativeState.HasFlag(NarrativeFlags.ChoiceMade))
            {
                return _narrativeState.HasFlag(NarrativeFlags.ChoiceFragmentA)
                    ? Line(_config?.ChoiceFragmentALine, "I remember leaving. It was my choice to go.")
                    : Line(_config?.ChoiceFragmentBLine, "I remember fading. I didn't want to go.");
            }

            if (_narrativeState.HasFlag(NarrativeFlags.WalkwayRebuilt))
            {
                return Line(_config?.WalkwayRebuiltLine, "The path is clear now. You're getting closer.");
            }

            // Check fragment order based on registry history
            if (_fragmentRegistry != null && _fragmentRegistry.CollectionHistory.Count > 0)
            {
                string lastFragment = _fragmentRegistry.CollectionHistory[_fragmentRegistry.CollectionHistory.Count - 1];
                string reaction = FindFragmentReaction(lastFragment);
                if (!string.IsNullOrEmpty(reaction)) return reaction;
            }

            return Line(_config?.DefaultLine, "You're awake. We have work to do if we're going to fix this place.");
        }

        private string FindFragmentReaction(string fragmentId)
        {
            if (_config == null || _config.FragmentReactions == null) return string.Empty;
            foreach (var pair in _config.FragmentReactions)
            {
                if (pair.FragmentId == fragmentId)
                    return pair.DialogueLine;
            }
            return string.Empty;
        }

        private static string Line(string configLine, string fallback)
            => string.IsNullOrEmpty(configLine) ? fallback : configLine;
    }
}
