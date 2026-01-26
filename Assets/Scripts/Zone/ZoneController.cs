using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Zone
{
    public class ZoneController : NetworkBehaviour
    {
        [SerializeField]
        private string _name;

        private Collider _collider;

        public string Name => _name;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            ZoneManager.Instance.RegisterZone(this);
        }

        public bool Contains(Transform transform)
        {
            return _collider.bounds.Contains(transform.position);
        }
    }
}
