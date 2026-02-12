using MMOJam.Manager;
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

            GameManager.Instance.CheckVictoryCondition();

            foreach (var p in ServerManager.Instance.GetDeadFactionPlayer(Faction))
            {
                p.TryRespawn();
            }

            foreach (var building in _buildings)
            {
                building.BuildingRestored(faction);
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
                var player = other.GetComponent<PlayerController>();
                player.SetZone(newZone);
                foreach (var bat in _buildings)
                {
                    if (newZone == null) bat.OnZoneExit(player);
                    else bat.OnZoneEnter(player);
                }
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
