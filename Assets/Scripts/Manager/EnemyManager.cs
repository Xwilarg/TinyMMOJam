using MMOJam.Player;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class EnemyManager : MonoBehaviour   
    {
        public static EnemyManager Instance { private set; get; }

        [SerializeField]
        private GameObject _playerPrefab;

        private SpawnPoint[] _spawnPoints;

        private void Awake()
        {
            Instance = this;
            _spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).Where(x => x.SpawnType == SpawnType.Enemy).ToArray();
        }

        public void SpawnWave(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var sp = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
                var go = Instantiate(_playerPrefab);
                go.layer = LayerMask.NameToLayer("MovingProp");
                //go.GetComponent<PlayerController>().IsAi = true;
                var no = go.GetComponent<NetworkObject>();
                no.Spawn();
                go.transform.position = sp.GetRandomPos();
            }
        }
    }
}
