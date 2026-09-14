using UnityEngine;

namespace EchoZero.World.Anchors
{
    /// <summary>
    /// Data definition for a Reconstruction Anchor.
    /// TASK: TASK-007
    /// </summary>
    [CreateAssetMenu(menuName = "EchoZero/Reconstruction Anchor", fileName = "Anchor_")]
    public class ReconstructionAnchorSO : ScriptableObject
    {
        [Tooltip("Unique ID for this anchor.")]
        public string anchorId;

        [Tooltip("List of fragment IDs required to rebuild this anchor.")]
        public string[] requiredFragmentIds;
    }
}
