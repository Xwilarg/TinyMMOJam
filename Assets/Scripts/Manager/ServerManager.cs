using MMOJam.Player;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class ServerManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private SpawnPoint[] _spawns;

        [SerializeField]
        private int _spawnsCount;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsHost && !IsServer)
            {
                Destroy(gameObject);
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
