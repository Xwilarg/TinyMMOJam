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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsAuthority) GameManager.Instance.InitNetwork();
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
            Debug.Log($"[SMI] added {elem.Key} total [{string.Join(", ", _interactibles.Keys)}]");
        }

        public void UnregisterInteractible(AInteractible elem)
        {
            if (elem == null)
                return;

            _interactibles.Remove(elem.Key);
            Debug.Log($"[SMI] removed {elem.Key} total [{string.Join(", ", _interactibles.Keys)}]");
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

        public int GetNextFaction()
        {
#if UNITY_EDITOR
            if (_factions.Any(f => f.Id == 0) || _factions.GroupBy(f => f.Id).Any(x => x.Count() > 1))
            {
                throw new System.InvalidOperationException("You should properly setup your SO");
            }
#endif

            // Get next faction with least players
            return _factions.OrderBy(f => _players.Count(p => p.CurrentFaction.Value == f.Id)).First().Id;
        }
    }
}
