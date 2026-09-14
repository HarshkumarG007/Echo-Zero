using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;

namespace EchoZero.Narrative
{
    /// <summary>
    /// ScriptableObject data asset for a single memory fragment.
    /// Create via: Assets → Create → EchoZero → Memory Fragment
    ///
    /// TASK: TASK-005
    /// </summary>
    [CreateAssetMenu(menuName = "EchoZero/Memory Fragment", fileName = "Fragment_")]
    public class MemoryFragmentSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique string ID. Used by NarrativeState and SaveService. Never change after first use.")]
        public string fragmentId;

        [Header("Content")]
        [Tooltip("ECHO voiceover audio clip for this fragment.")]
        public AudioClip echoVoiceoverClip;

        [Header("Narrative")]
        [Tooltip("Anchor IDs this fragment contributes to. May be empty if fragment is standalone.")]
        public string[] contributesToAnchorIds;

        [Tooltip("Fragment ID this fragment contradicts. Leave empty if no contradiction.")]
        public string contradictsFragmentId;

        [Tooltip("Narrative flag key set when this fragment is collected.")]
        public string narrativeFlagOnCollect;

        [Header("Debug")]
        [Tooltip("Human-readable description for editor reference only.")]
        [Multiline]
        public string editorNotes;
    }
}
