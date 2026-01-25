using UnityEngine;
using UnityEngine.Events;

namespace MMOJam.Player
{
    public class TriggerArea : MonoBehaviour
    {
        private UnityEvent<Collider> OnTriggerEnterEvent { get; } = new();
        private UnityEvent<Collider> OnTriggerExitEvent { get; } = new();

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent.Invoke(other);
        }
    }
}
