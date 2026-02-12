using MMOJam.Player;
using MMOJam.SO;
using MMOJam.Vehicle;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager Instance { private set; get; }

        [SerializeField]
        private FactionInfo[] _factions;

        [SerializeField]
        private UIDocument _ui;

        [SerializeField]
        private CinemachineCamera _cam;

        private Dictionary<ulong, AInteractible> _interactibles = new();
        private readonly List<PlayerController> _players = new();
        private PlayerController _me = null;

        private NetworkManager _manager;

        public bool IsAuthority => IsHost || IsServer;

        private void Awake()
        {
            Instance = this;
            _manager = GetComponent<NetworkManager>();

            _ui.rootVisualElement.Q<VisualElement>("disconnected").visible = false;
            _ui.rootVisualElement.Q<Button>("disconnected-menu").clicked += () =>
            {
                SceneManager.LoadScene("Menu");
            };
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsAuthority) GameManager.Instance.InitNetwork();

            foreach (var player in _players)
            {
                if (player.IsOwner) _me = player;
            }

            _cam.Lens.OrthographicSize = 7.5f;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            SceneManager.LoadScene("Menu");
        }

        public void RegisterPlayer(PlayerController player) => _players.Add(player);
        public void UnregisterPlayer(PlayerController player) => _players.Remove(player);

        public bool AreOthersFactionDead(FactionInfo faction)
            => GetAllOtherFactionPlayer(faction).All(x => !x.IsAlive);

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

        public PlayerController GetNetworkPlayer(ulong PlayerNetworkId)
        {
            foreach (var player in _players)
            {
                if (player.NetworkObjectId == PlayerNetworkId) return player;
            }
            return null;
        }

        public PlayerController GetLocalPlayer()
        {
            foreach (var player in _players)
            {
                if (player.IsOwner) return player;
            }
            return null;
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

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestCraftServerRpc(ulong PlayerNetworkId, short recipeId)
        {
            CraftingManager.Instance.CraftRecipe(PlayerNetworkId, recipeId);
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

        public IEnumerable<PlayerController> GetAllOtherFactionPlayer(FactionInfo faction)
        {
            foreach (var player in _players)
            {
                if (!player.IsAi && player.CurrentFaction.Value != faction.Id) yield return player;
            }
        }

        public IEnumerable<PlayerController> GetDeadFactionPlayer(FactionInfo faction)
        {
            foreach (var player in _players)
            {
                if (player.CurrentFaction.Value == faction.Id && !player.IsAlive) yield return player;
            }
        }
    }
}
