using System.Collections.Generic;

namespace EchoZero.Core.WorldState
{
    /// <summary>
    /// Tracks which objects in the world hold which memory fragments.
    /// TASK: TASK-004
    /// </summary>
    public class WorldState
    {
        private readonly Dictionary<string, string> _objectToFragmentId = new Dictionary<string, string>();

        public void RegisterFragment(string objectId, string fragmentId)
        {
            if (string.IsNullOrEmpty(objectId)) return;
            _objectToFragmentId[objectId] = fragmentId;
        }

        public void UnregisterFragment(string objectId)
        {
            if (string.IsNullOrEmpty(objectId)) return;
            _objectToFragmentId.Remove(objectId);
        }

        public bool TryGetFragment(string objectId, out string fragmentId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                fragmentId = null;
                return false;
            }
            return _objectToFragmentId.TryGetValue(objectId, out fragmentId);
        }
    }
}
