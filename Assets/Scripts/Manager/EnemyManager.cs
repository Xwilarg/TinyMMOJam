using MMOJam.Player;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class EnemyManager : NetworkBehaviour
    {
        [SerializeField]
        private SpawnPoint[] _spawns;

        [SerializeField]
        private int _spawnsCount;

        [SerializeField]
        private GameObject _playerPrefab;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!ServerManager.Instance.IsAuthority)
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
