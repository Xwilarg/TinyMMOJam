using UnityEngine;

namespace MMOJam.Player
{
    public class NetworkPlayerInstance : MonoBehaviour
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void UpdateVelocity(Vector3 adjustedPosition, Vector3 newVelocity)
        {
            _rb.linearVelocity = newVelocity;
            transform.position = adjustedPosition;
        }
    }
}
