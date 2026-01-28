using MMOJam.Player;
using MMOJam.SO;
using MMOJam.Vehicle;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager Instance { private set; get; }

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private SpawnPoint[] _spawns;

        [SerializeField]
        private int _spawnsCount;

        [SerializeField]
        private FactionInfo[] _factions;

        private Dictionary<ulong, AInteractible> _interactibles = new();
        private readonly List<PlayerController> _players = new();

        private NetworkManager _manager;

        public bool IsAuthority => IsHost || IsServer;

        private void Awake()
        {
            Instance = this;
            _manager = GetComponent<NetworkManager>();
        }

        public void RegisterPlayer(PlayerController player) => _players.Add(player);
        public void UnregisterPlayer(PlayerController player) => _players.Remove(player);

        public IEnumerable<PlayerController> GetPlayersInVehicle(ulong vehicleId)
        {
            lock (_players)
            {
                foreach (var player in _players)
                {
                    if (player.CurrentVehicle.Value == vehicleId) yield return player;
                }
            }
        }

        public void RegisterInteractible(AInteractible elem)
        {
            _interactibles.Add(elem.Key, elem);
        }

        public void InteractWith(ulong key, PlayerController player)
        {
            if (_interactibles.TryGetValue(key, out var i)) i.InteractClient(player);
        }

        public RuntimeVehicle GetVehicle(ulong key)
        {
            return _interactibles[key] as RuntimeVehicle;
        }

        public FactionInfo GetFaction(int factionId)
        {
            return _factions.FirstOrDefault(f => f.Id == factionId);
        }

        public FactionInfo GetNextFaction()
        {
#if DEBUG
            if (_factions.Any(f => f.Id == 0) || _factions.GroupBy(f => f.Id).Any(x => x.Count() > 1))
            {
                throw new System.InvalidOperationException("You should properly setup your SO");
            }
#endif

            // Get next faction with least players
            return _factions.OrderBy(f => _players.Count(p => p.CurrentFaction.Value == f.Id)).First();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsAuthority)
            {
                return;
            }

            for (int i = 0; i < _spawnsCount; i++)
            {
                var sp = _spawns[Random.Range(0, _spawnsCount - 1)];
                var go = Instantiate(_playerPrefab);
                go.layer = LayerMask.NameToLayer("MovingProp");
                go.GetComponent<PlayerController>().IsAi = true;
                var no = go.GetComponent<NetworkObject>();
                no.Spawn();
                go.transform.position = sp.GetRandomPos();
            }
        }
    }
}
