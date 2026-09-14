using UnityEngine;
using EchoZero.Core;
using EchoZero.Narrative.Fragments;

namespace EchoZero.Narrative.Mira
{
    /// <summary>
    /// Deterministically selects the appropriate dialogue line for Mira
    /// based on the current NarrativeState and FragmentRegistry state.
    /// TASK: TASK-008
    /// </summary>
    public class MiraDialogueSelector
    {
        private readonly NarrativeState _narrativeState;
        private readonly FragmentRegistry _fragmentRegistry;

        public MiraDialogueSelector(NarrativeState narrativeState, FragmentRegistry fragmentRegistry)
        {
            _narrativeState = narrativeState;
            _fragmentRegistry = fragmentRegistry;
        }

        /// <summary>
        /// Returns the current dialogue line Mira should speak.
        /// </summary>
        public string GetCurrentDialogue()
        {
            if (_narrativeState.HasFlag(NarrativeFlags.RevealTriggered))
            {
                // Post-reveal, she is silent/gone.
                return "";
            }

            if (_fragmentRegistry != null && _fragmentRegistry.IsPlayerMustChooseState)
            {
                // Active contradiction
                return "Two memories. They cannot both be true. You must decide what happened.";
            }

            if (_narrativeState.HasFlag(NarrativeFlags.ChoiceMade))
            {
                if (_narrativeState.HasFlag(NarrativeFlags.ChoiceFragmentA))
                {
                    return "I remember leaving. It was my choice to go.";
                }
                else
                {
                    return "I remember fading. I didn't want to go.";
                }
            }

            if (_narrativeState.HasFlag(NarrativeFlags.WalkwayRebuilt))
            {
                return "The path is clear now. You're getting closer.";
            }

            // Check fragment order based on registry history
            if (_fragmentRegistry != null && _fragmentRegistry.CollectionHistory.Count > 0)
            {
                string lastFragment = _fragmentRegistry.CollectionHistory[_fragmentRegistry.CollectionHistory.Count - 1];
                if (lastFragment == "Fragment_A") return "That memory... it feels heavy. Are you sure it's yours?";
                if (lastFragment == "Fragment_B") return "A bright shard. It almost hurts to look at.";
                if (lastFragment == "Fragment_01") return "You found a piece of it. There is more out there.";
            }

            if (_narrativeState.HasFlag(NarrativeFlags.Fragment01Collected))
            {
                return "You found a piece of it. There is more out there.";
            }

            // Default opening line
            return "You're awake. We have work to do if we're going to fix this place.";
        }
    }
}
