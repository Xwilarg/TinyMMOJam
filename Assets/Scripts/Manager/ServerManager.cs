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

        private Dictionary<EntityId, AInteractible> _interactibles = new();

        public bool IsAuthority => IsHost || IsServer;

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterInteractible(AInteractible elem)
        {
            _interactibles.Add(elem.gameObject.GetEntityId(), elem);
        }

        public void InteractWith(EntityId entity, PlayerController player)
        {
            Debug.Log(entity);
            Debug.Log(_interactibles.Count);
            if (_interactibles.TryGetValue(entity, out var i)) i.Interact(player);
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
