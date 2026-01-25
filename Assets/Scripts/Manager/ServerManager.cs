using MMOJam.Player;
using System.Collections.Generic;
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

        private Dictionary<ulong, AInteractible> _interactibles = new();

        public bool IsAuthority => IsHost || IsServer;

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterInteractible(AInteractible elem)
        {
            _interactibles.Add(elem.Key, elem);
        }

        public void InteractWith(ulong key, PlayerController player)
        {
            if (_interactibles.TryGetValue(key, out var i)) i.Interact(player);
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
                go.GetComponent<PlayerController>().IsAi = true;
                var no = go.GetComponent<NetworkObject>();
                no.Spawn();
                go.transform.position = sp.GetRandomPos();
            }
        }
    }
}
