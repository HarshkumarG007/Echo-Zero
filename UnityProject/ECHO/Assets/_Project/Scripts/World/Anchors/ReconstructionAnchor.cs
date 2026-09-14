using UnityEngine;
using EchoZero.Core;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;
using EchoZero.Gameplay.Recall;
using EchoZero.Narrative.Fragments;

namespace EchoZero.World.Anchors
{
    /// <summary>
    /// World object that can be rebuilt using RECALL if the required fragments are collected.
    /// TASK: TASK-007
    /// </summary>
    public class ReconstructionAnchor : MonoBehaviour, IRecallable
    {
        [SerializeField] private ReconstructionAnchorSO _anchorData;
        [SerializeField] private GameObject _visualTarget;
        
        private bool _isRebuilt;

        public bool CanRecall => !_isRebuilt && _anchorData != null;

        public string ObjectId => gameObject.GetInstanceID().ToString();

        public void OnRecall()
        {
            if (_isRebuilt || _anchorData == null) return;

            var registry = ServiceLocator.Get<FragmentRegistry>();
            if (registry == null)
            {
                Debug.LogWarning("[ReconstructionAnchor] FragmentRegistry not found.");
                return;
            }

            bool allCollected = true;
            if (_anchorData.requiredFragmentIds != null)
            {
                foreach (var fragId in _anchorData.requiredFragmentIds)
                {
                    if (!registry.IsCollected(fragId))
                    {
                        allCollected = false;
                        break;
                    }
                }
            }

            if (allCollected)
            {
                _isRebuilt = true;
                
                if (_visualTarget != null)
                {
                    _visualTarget.SetActive(true);
                }

                EventBus<AnchorRebuiltEvent>.Publish(new AnchorRebuiltEvent
                {
                    AnchorId = _anchorData.anchorId
                });

                Debug.Log($"[ReconstructionAnchor] Anchor {_anchorData.anchorId} rebuilt successfully.");
            }
            else
            {
                Debug.Log($"[ReconstructionAnchor] Missing required fragments for {_anchorData.anchorId}. RECALL failed.");
            }
        }
    }
}
