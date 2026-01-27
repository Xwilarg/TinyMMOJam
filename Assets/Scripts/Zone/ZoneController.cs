using MMOJam.Player;
using MMOJam.SO;
using MMOJam.Vehicle;
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

        private void OnTriggerEnter(Collider other)
        {
            OnEnterExit(other, this);
        }

        private void OnTriggerExit(Collider other)
        {
            OnEnterExit(other, null);
        }

        private void OnEnterExit(Collider other, ZoneController newZone)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().SetZone(newZone);
            }

            if (other.CompareTag("Vehicle"))
            {
                var vehicle = other.GetComponent<RuntimeVehicle>();

                foreach (var player in vehicle.GetPlayersInVehicle())
                {
                    player.SetZone(newZone);
                }
            }
        }

        public bool Contains(Transform transform)
        {
            return _collider.bounds.Contains(transform.position);
        }
    }
}
