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
        private FactionInfo _defaultFaction;

        private NetworkVariable<int> _factionId = new(0);

        private Collider _collider;

        public FactionInfo Faction => ServerManager.Instance.GetFaction(_factionId.Value);

        private ABuilding[] _buildings;

        private void Awake()
        {
            if (IsServer || IsHost) _factionId.Value = _defaultFaction.Id;

            _collider = GetComponent<Collider>();
            _buildings = GetComponentsInChildren<ABuilding>();
            /*
            _factionId.OnValueChanged += (int oldValue, int newValue) =>
            {
                foreach (var building in _buildings)
                {
                    building.AttachedZone = this;
                    building.UpdateFactionData();
                }
            };*/
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log($"[FAC] Update attached zone data (current faction is {_factionId.Value})");
            foreach (var building in _buildings)
            {
                building.AttachedZone = this;
                building.UpdateFactionData();
            }
        }

        public void ConvertZoneTo(FactionInfo faction)
        {
            _factionId.Value = faction.Id;

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
                    foreach (var bat in _buildings)
                    {
                        if (newZone == null) bat.OnZoneExit(player);
                        else bat.OnZoneEnter(player);
                    }
                }
            }
        }

        public void SpawnPlayer(PlayerController player)
        {
            GetComponentInChildren<SpawnBuilding>().SpawnPlayer(player);
            foreach (var building in _buildings)
            {
                building.SpawnPlayer();
            }
        }

        public bool Contains(Transform transform)
        {
            return _collider.bounds.Contains(transform.position);
        }
    }
}
