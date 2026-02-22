using MMOJam.Player;
using MMOJam.SO;
using MMOJam.Vehicle;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MMOJam.Manager
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager Instance { private set; get; }

        private Dictionary<ulong, AInteractible> _interactibles = new();
        private readonly List<PlayerController> _players = new();
        private PlayerController _me = null;

        private NetworkManager _manager;

        public bool IsAuthority => IsHost || IsServer;

        [Rpc(SendTo.ClientsAndHost)]
        public void TakeDamageAtRpc(Vector3 pos, int damage)
        {
            var bullet = Instantiate(GameManager.Instance.BulletPrefab, pos, Quaternion.identity);
            bullet.GetComponent<TMP_Text>().text = (-damage).ToString();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DispatchPsaRpc(string message, string subtitle)
        {
            UIManager.Instance.DispatchPsa(message, subtitle);
        }

        public void TakeDamageAt(Vector3 pos, int damage)
        {
            if (IsServer)
            {
                TakeDamageAtRpc(pos, damage);
            }
            else
            {
                // Shouldn't happen?
                var bullet = Instantiate(GameManager.Instance.BulletPrefab, pos, Quaternion.identity);
                bullet.GetComponent<TMP_Text>().text = (-damage).ToString();
            }
        }
        private void Start()
        {
            Instance = this;
            _manager = GetComponent<NetworkManager>();

            UIManager.Instance.UI.rootVisualElement.Q<VisualElement>("disconnected").visible = false;
            UIManager.Instance.UI.rootVisualElement.Q<Button>("disconnected-menu").clicked += () =>
            {
                SceneManager.LoadScene("Main");
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

            UIManager.Instance.Cam.Lens.OrthographicSize = 7.5f;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            SceneManager.LoadScene("Main");
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
            return UIManager.Instance.Factions.FirstOrDefault(f => f.Id == factionId);
        }

        public int GetNextFaction()
        {
#if UNITY_EDITOR
            if (/*UIManager.Instance.Factions.Any(f => f.Id == 0) || */UIManager.Instance.Factions.GroupBy(f => f.Id).Any(x => x.Count() > 1))
            {
                throw new System.InvalidOperationException("You should properly setup your SO");
            }
#endif

            // Get next faction with least players
            return UIManager.Instance.Factions.OrderBy(f => _players.Count(p => p.CurrentFaction.Value == f.Id)).First().Id;
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
