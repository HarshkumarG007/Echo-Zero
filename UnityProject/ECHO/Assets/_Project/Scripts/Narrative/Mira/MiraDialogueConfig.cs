using System;
using UnityEngine;

namespace EchoZero.Narrative.Mira
{
    [Serializable]
    public class FragmentDialoguePair
    {
        [Tooltip("The fragment ID this dialogue responds to.")]
        public string FragmentId;
        [TextArea(2, 4)]
        public string DialogueLine;
    }

    /// <summary>
    /// ScriptableObject holding all of Mira's configurable dialogue lines.
    /// Designer-editable in the Unity Inspector — no code changes needed to add lines.
    /// Create via: Assets > Create > EchoZero > Mira Dialogue Config
    /// TASK: TASK-020 (ARCH-002 fix)
    /// </summary>
    [CreateAssetMenu(fileName = "MiraDialogueConfig", menuName = "EchoZero/Mira Dialogue Config")]
    public class MiraDialogueConfig : ScriptableObject
    {
        [Header("Default")]
        [TextArea(2, 4)]
        public string DefaultLine = "You're awake. We have work to do if we're going to fix this place.";

        [Header("State-based overrides")]
        [TextArea(2, 4)]
        public string ContradictionLine = "Two memories. They cannot both be true. You must decide what happened.";
        [TextArea(2, 4)]
        public string WalkwayRebuiltLine = "The path is clear now. You're getting closer.";
        [TextArea(2, 4)]
        public string ChoiceFragmentALine = "I remember leaving. It was my choice to go.";
        [TextArea(2, 4)]
        public string ChoiceFragmentBLine = "I remember fading. I didn't want to go.";

        [Header("Fragment-specific reactions (last collected)")]
        public FragmentDialoguePair[] FragmentReactions;
    }
}
