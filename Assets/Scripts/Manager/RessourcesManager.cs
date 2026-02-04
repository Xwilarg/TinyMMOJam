using MMOJam.Player;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace MMOJam.Manager
{
    public class RessourcesManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        private SpawnPoint[] _spawnPoints;

        public static RessourcesManager Instance { private set; get; }
        private Dictionary<ulong, RessourcesHolder> _ressources_holders = new();

        private void Awake()
        {
            Instance = this;

            _spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).Where(x => x.SpawnType == SpawnType.Resource).ToArray();
        }
        public void RegisterHolder(RessourcesHolder elem)
        {
            _ressources_holders.Add(elem.GetComponent<NetworkObject>().NetworkObjectId, elem);
            Debug.Log($"[RHL] added {elem.GetComponent<NetworkObject>().NetworkObjectId} total [{string.Join(", ", _ressources_holders.Keys)}]");
        }

        public void DeleteHolder(ulong id)
        {
            if (_ressources_holders.Remove(id))
            {
                Debug.Log($"[RHL] deleted {id} total [{string.Join(", ", _ressources_holders.Keys)}]");
            }
            else
            {
                Debug.LogWarning($"[RHL] tried to delete {id} but it was not found");
            }
        }
    }
}