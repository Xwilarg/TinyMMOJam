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
        private FactionInfo _faction;

        private Collider _collider;

        public FactionInfo Faction => _faction;

        private ABuilding[] _buildings;

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            _buildings = GetComponentsInChildren<ABuilding>();
            foreach (var building in _buildings)
            {
                building.AttachedZone = this;
                building.UpdateFactionData();
            }
        }

        public void ConvertZoneTo(FactionInfo faction)
        {
            _faction = faction;

            foreach (var building in _buildings)
            {
                building.Restore();
                building.UpdateFactionData();
            }
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
            if (!IsHost && !IsServer) return;

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

        public void SpawnPlayer(PlayerController player)
        {
            GetComponentInChildren<SpawnBuilding>().SpawnPlayer(player);
        }

        public bool Contains(Transform transform)
        {
            return _collider.bounds.Contains(transform.position);
        }
    }
}
