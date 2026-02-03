using MMOJam.Player;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class EnemyManager : NetworkBehaviour
    {
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

            var spawns = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).Where(x => x.SpawnType == SpawnType.Enemy).ToArray();

            for (int i = 0; i < _spawnsCount; i++)
            {
                var sp = spawns[Random.Range(0, spawns.Length)];
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
