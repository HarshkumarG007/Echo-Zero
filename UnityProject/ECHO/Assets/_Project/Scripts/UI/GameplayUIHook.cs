using UnityEngine;
using EchoZero.Core.Events;
using EchoZero.Core.Events.Narrative;

namespace EchoZero.UI
{
    [RequireComponent(typeof(GameplayUI))]
    public class GameplayUIHook : MonoBehaviour
    {
        private GameplayUI _ui;

        private void Awake()
        {
            _ui = GetComponent<GameplayUI>();
            EventBus<FragmentCollectedEvent>.Subscribe(OnFragmentCollected);
        }

        private void OnDestroy()
        {
            EventBus<FragmentCollectedEvent>.Unsubscribe(OnFragmentCollected);
        }

        private void OnFragmentCollected(FragmentCollectedEvent evt)
        {
            _ui.ShowDialogue($"Memory {evt.FragmentId} restored...\nThe grief forms the foundation.");
            Invoke(nameof(HideDialogue), 5f);
        }

        private void HideDialogue()
        {
            _ui.HideDialogue();
        }
    }
}
